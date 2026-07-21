using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;
using SESMAN.Server;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class RestRequestService : IRestRequestService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRequestLogRepository _requestLogRepository;
    private readonly IMapper _mapper;

    public RestRequestService(
        IHttpClientFactory httpClientFactory,
        IRequestLogRepository requestLogRepository,
        IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _requestLogRepository = requestLogRepository;
        _mapper = mapper;
    }

    public async Task<RequestLogDto> ExecuteAndSaveRequestAsync(CreateRequestLogDto dto)
    {
        var requestLog = _mapper.Map<RequestLog>(dto);
        var client = _httpClientFactory.CreateClient();

        // 10 saniye içinde dönüş olmazsa time out vermesi için
        client.Timeout = TimeSpan.FromSeconds(10);

        var httpRequest = new HttpRequestMessage(new HttpMethod(dto.Method.ToString()), dto.Url);

        // GET dışındaki isteklere seçilen BodyType'a göre content eklenmesi
        if (!string.IsNullOrWhiteSpace(dto.Body) && dto.Method.ToString() != "GET")
        {
            // Eğer Vue'dan BodyType gelmezse varsayılan olarak 'raw' kabul et
            string bodyType = string.IsNullOrWhiteSpace(dto.BodyType) ? "raw" : dto.BodyType;

            switch (bodyType)
            {
                case "raw":
                case "GraphQL":
                    // Vue'dan gelen Header listesinde "Content-Type" var mı diye bakıyoruz
                    var contentType = dto.RequestHeaders
                        .FirstOrDefault(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))?.Value
                        ?? "text/plain"; // Eğer bulamazsa varsayılan olarak text/plain olsun

                   
                    httpRequest.Content = new StringContent(dto.Body, Encoding.UTF8, contentType);
                    break;

                case "x-www-form-urlencoded":
                    
                    var urlEncodedList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(dto.Body);
                    var keyValues = new List<KeyValuePair<string, string>>();

                    if (urlEncodedList != null)
                    {
                        foreach (var item in urlEncodedList)
                        {
                            if (item.TryGetValue("key", out var key) && item.TryGetValue("value", out var val))
                            {
                                keyValues.Add(new KeyValuePair<string, string>(key, val));
                            }
                        }
                    }
                    httpRequest.Content = new FormUrlEncodedContent(keyValues);
                    break;


                case "binary":
                    // Vue'dan Base64 string olarak gelen body'yi geri byte dizisine çeviriyoruz
                    byte[] fileBytes = Convert.FromBase64String(dto.Body);

                    // Bunu bir ByteArrayContent'e sarıyoruz (Ham dosya gönderimi)
                    httpRequest.Content = new ByteArrayContent(fileBytes);

                    // Genellikle binary dosyalarda hedef siteye "Sana ham bir dosya yolluyorum" demek için octet-stream header'ı verilir (böyle kullanılıyormuş)
                    httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    break;



                case "form-data":
                    // Multipart Form Data oluşturuyoruz
                    var multipartContent = new MultipartFormDataContent();

                    // Vue'dan List<Dictionary> olarak gelen veriyi çözüyoruz (key, value, type içeriyor)
                    var formDataList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(dto.Body);

                    if (formDataList != null)
                    {
                        foreach (var item in formDataList)
                        {
                            if (item.TryGetValue("key", out var key) && item.TryGetValue("value", out var val))
                            {
                                // Vue'dan type değeri gelmiş mi kontrol et, gelmediyse varsayılan "text" say
                                string type = item.TryGetValue("type", out var t) ? t : "text";

                                if (type == "file")
                                {
                                    // Dosya işlemleri: Base64 string'i byte dizisine çeviriyoruz
                                    if (!string.IsNullOrEmpty(val))
                                    {
                                        byte[] fromBytes = Convert.FromBase64String(val);
                                        var fileContent = new ByteArrayContent(fromBytes);

                                        // Multipart formData'da dosya eklerken key ve dosya adı parametreleri gerekir
                                        // Şimdilik dosya adına "uploaded_file" olarak alıyorum. 
                                        multipartContent.Add(fileContent, key, "uploaded_file");
                                    }
                                }
                                else
                                {
                                    // Normal metin işlemleri
                                    multipartContent.Add(new StringContent(val), key);
                                }
                            }
                        }
                    }
                    httpRequest.Content = multipartContent;
                    break;

                case "none":
                default:
                    // Body yok
                    httpRequest.Content = null;
                    break;

              
            }
        }
        // Gönderilen headerların bilgilerini isteğe ekleme
        if (dto.RequestHeaders != null)
        {
            foreach (var header in dto.RequestHeaders)
            {
                if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                //normal add ile yapılsaydı standart dışı karakterlerde hata alırdık, onun yerine kontrolsüz geçirip istek atılan sunucudan dönüş alınması sağlanıyor.
                httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var httpResponse = await client.SendAsync(httpRequest);
            stopwatch.Stop();

            var responseBody = await httpResponse.Content.ReadAsStringAsync();

            requestLog.Response = new ResponseLog
            {
                StatusCode = (int)httpResponse.StatusCode,
                Body = responseBody,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                ResponseHeaders = new List<ResponseHeader>()
            };

            // Genel Response Header'larını yakalar
            foreach (var header in httpResponse.Headers)
            {
                requestLog.Response.ResponseHeaders.Add(new ResponseHeader
                {
                    Key = header.Key,
                    Value = string.Join(", ", header.Value)
                });
            }

            // İçerik Header'larını yakalar
            if (httpResponse.Content?.Headers != null)
            {
                foreach (var header in httpResponse.Content.Headers)
                {
                    requestLog.Response.ResponseHeaders.Add(new ResponseHeader
                    {
                        Key = header.Key,
                        Value = string.Join(", ", header.Value)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            string errorMessage = $"System Error: {ex.Message}";

            //hazır şekilde olan InnerException ile hatanın detaylarını alabiliyoruz. (System.Exception)
            if (ex.InnerException != null)
            {
                errorMessage += $" | Details: {ex.InnerException.Message}";
            }

            // HttpClient, Timeout (Zaman Aşımı) durumunda TaskCanceledException fırlatır.
            if (ex is TaskCanceledException)
            {
                errorMessage = "System Error: The request timed out after 10 seconds.";
            }

            requestLog.Response = new ResponseLog
            {
                StatusCode = 0, // hedefe hiç ulaşılmadıysa
                Body = errorMessage,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                ResponseHeaders = new List<ResponseHeader>()
            };
        }

        await _requestLogRepository.AddAsync(requestLog);

        return _mapper.Map<RequestLogDto>(requestLog);
    }
}