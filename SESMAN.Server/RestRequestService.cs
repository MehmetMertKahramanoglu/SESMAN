using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;
using SESMAN.Server;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
        //maplemeyi başta yapma sebebimiz her türlü loglanacak olması başarısız bile olsa bu veriler loglanmalı
        var requestLog = _mapper.Map<RequestLog>(dto);//IntegrationController.cs yollanan CreateRequestLogDto türündeki dto burada maplenerek RequestLog türüne çevrilerek requestLog değişkenine atanır.

        using var client = _httpClientFactory.CreateClient(); //.Net sınıfıdır ve dış sitelere istek atmak için client üretilir.
        client.Timeout = TimeSpan.FromSeconds(10); //hedef sunucu 10 saniye içinde cevap vermezse TimeOut olur 


        //isteği oluştur (body ve headerlarla beraber)
        using var httpRequest = BuildHttpRequest(dto);

        //response Time için kronometre başlatılır.
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var httpResponse = await client.SendAsync(httpRequest); //await olduğu için dönüş olana kadar kod burada bekler ve SendAsync ile hedefe istek atılır ve gelen cevap httpResponse'a atılır.
            //cevap geldiği an kronometre durdurulur.
            stopwatch.Stop();


            requestLog.Response = await ParseSuccessResponseAsync(httpResponse, stopwatch.ElapsedMilliseconds);
        }
        //10 saniyelik TimeOut zamanı dolduğunda veya bir sorun olduğunda uygulama çökmesin diye
        catch (Exception ex)
        {
            //kronometre durdurulur.
            stopwatch.Stop();

            requestLog.Response = ParseErrorResponse(ex, stopwatch.ElapsedMilliseconds);
        }

        //db ye kayıt
        await _requestLogRepository.AddAsync(requestLog); //RequestLogRepository içindeki AddAsync ie db'ye kayıt ederiz.
        return _mapper.Map<RequestLogDto>(requestLog); //Burada maplemeyle RequestLogDto türündeki requestLog tekrardan CreateRequestLogDto türüne döndürülür ve IntegrationController.cs tarafına return edilir (oradanda vue tarafına döneceğiz.).
    }


    private HttpRequestMessage BuildHttpRequest(CreateRequestLogDto dto)
    {
        var request = new HttpRequestMessage(new HttpMethod(dto.Method.ToString()), dto.Url);

        // Body ekleme işlemi
        if (!string.IsNullOrWhiteSpace(dto.Body) && dto.Method.ToString() != "GET")
        {
            request.Content = CreateHttpContent(dto);
        }

        //header eklenmesi öncesi Authorization kısmının eklenmesi için çağırma
        ApplyAuthorization(request, dto.Auth);

        // Header ekleme işlemi
        if (dto.RequestHeaders != null)
        {
            foreach (var header in dto.RequestHeaders)
            {
                if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    continue;

                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return request;
    }

    private HttpContent? CreateHttpContent(CreateRequestLogDto dto)
    {
        string bodyType = string.IsNullOrWhiteSpace(dto.BodyType) ? "raw" : dto.BodyType;

        return bodyType switch
        {
            "raw" or "GraphQL" => CreateRawContent(dto),
            "x-www-form-urlencoded" => CreateUrlEncodedContent(dto.Body ?? string.Empty),
            "binary" => CreateBinaryContent(dto.Body ?? string.Empty),
            "form-data" => CreateFormDataContent(dto.Body ?? string.Empty),
            _ => null
        };
    }

    private HttpContent CreateRawContent(CreateRequestLogDto dto)
    {
        // header listesinde content type'ları aramak için (büyük küçük harf duyarlılığını kaldırmak için StringComparison.OrdinalIgnoreCase)
        var contentType = dto.RequestHeaders
            ?.FirstOrDefault(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))?.Value
            ?? "text/plain";

        return new StringContent(dto.Body ?? string.Empty, Encoding.UTF8, contentType);
    }

    private HttpContent CreateUrlEncodedContent(string body)
    {
        var urlEncodedList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(body);
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
        return new FormUrlEncodedContent(keyValues);
    }

    private HttpContent CreateBinaryContent(string body)
    {
        byte[] fileBytes = Convert.FromBase64String(body);
        var content = new ByteArrayContent(fileBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        return content;
    }

    private HttpContent CreateFormDataContent(string body)
    {
        var multipartContent = new MultipartFormDataContent();
        var formDataList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(body);

        if (formDataList == null) return multipartContent;

        foreach (var item in formDataList)
        {
            if (item.TryGetValue("key", out var key) && item.TryGetValue("value", out var val))
            {
                string type = item.TryGetValue("type", out var t) ? t : "text";

                if (type == "file" && !string.IsNullOrEmpty(val))
                {
                    byte[] fromBytes = Convert.FromBase64String(val);
                    var fileContent = new ByteArrayContent(fromBytes);
                    multipartContent.Add(fileContent, key, "uploaded_file");
                }
                else
                {
                    multipartContent.Add(new StringContent(val), key);
                }
            }
        }
        return multipartContent;
    }

    private async Task<ResponseLog> ParseSuccessResponseAsync(HttpResponseMessage httpResponse, long executionTime)
    {
        var responseBody = await httpResponse.Content.ReadAsStringAsync();
        var responseLog = new ResponseLog
        {
            StatusCode = (int)httpResponse.StatusCode,
            Body = responseBody,
            ExecutionTimeMs = executionTime,
            ResponseHeaders = new List<ResponseHeader>()
        };

        // Genel Response Header'ları
        foreach (var header in httpResponse.Headers)
        {
            responseLog.ResponseHeaders.Add(new ResponseHeader
            {
                Key = header.Key,
                Value = string.Join(", ", header.Value)
            });
        }

        // İçerik (Content) Header'ları
        if (httpResponse.Content?.Headers != null)
        {
            foreach (var header in httpResponse.Content.Headers)
            {
                responseLog.ResponseHeaders.Add(new ResponseHeader
                {
                    Key = header.Key,
                    Value = string.Join(", ", header.Value)
                });
            }
        }

        return responseLog;
    }

    private ResponseLog ParseErrorResponse(Exception ex, long executionTime)
    {
        string errorMessage = ex switch
        {
            TaskCanceledException => "System Error: The request timed out after 10 seconds.",
            _ => $"System Error: {ex.Message}" + (ex.InnerException != null ? $" | Details: {ex.InnerException.Message}" : "")
        };

        return new ResponseLog
        {
            StatusCode = 0,
            Body = errorMessage,
            ExecutionTimeMs = executionTime,
            ResponseHeaders = new List<ResponseHeader>()
        };
    }
    

    private void ApplyAuthorization(HttpRequestMessage request, AuthConfigDto? auth)
    {   
        //burada auth gelmeme durumları için fonksiyona girmemesini sağladım.
        if (auth == null || string.IsNullOrWhiteSpace(auth.Type) || auth.Type.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        switch (auth.Type.ToLowerInvariant())
        {
            case "basic":
                if(!string.IsNullOrEmpty(auth.Username) && !string.IsNullOrEmpty(auth.Password)) //kullanıcı adı ve şifre null mı kontrolü.
                {
                    //girilen kullanıcı adı ve şifreyi username:password şeklinde birleştirme için
                    var authBytes = Encoding.UTF8.GetBytes($"{auth.Username}:{auth.Password}");
                    //birleştirilen username ve password'u Base64 ile şifreleyip Authorization Header'a ekle. (formatın doğru olması için C# sınıfı olan AuthenticationHeaderValue kullandım.)
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                }
                break;

            case "bearer":
            case "oauth2":
                if (!string.IsNullOrEmpty(auth.Token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.Token);
                }
                break;

            case "apikey":
                if(!string.IsNullOrEmpty(auth.ApiKeyName) && !string.IsNullOrEmpty(auth.ApiKeyValue))
                {
                    if (auth.ApiKeyAddTo?.ToLower() == "query")
                    {
                        ApplyApiKeyToQuery(request, auth.ApiKeyName, auth.ApiKeyValue);
                    }
                    else
                    {
                        request.Headers.TryAddWithoutValidation(auth.ApiKeyName, auth.ApiKeyValue);
                    }
                }
                break;
        }   
    }

    private void ApplyApiKeyToQuery(HttpRequestMessage request, string keyName, string keyValue)
    {
        //gidilecek adresin olmadığı durumda çalışmaması için.
        if(request.RequestUri == null)
        {
            return;
        }
        //URL'yi düzenlemek için UriBuilder oluştur.
        var uriBuilder = new UriBuilder(request.RequestUri); //UriBuilder ile hatasız şekilde URL birleşimi yapılır.
        //Mevcut query string'i alıp parse et.
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

        //yeni parametre ekle
        query[keyName] = keyValue;

        //güncellenmiş query string'i UriBuilder'a ata
        uriBuilder.Query = query.ToString();

        //istekteki URL'yi güncelle
        request.RequestUri = uriBuilder.Uri;
}
}