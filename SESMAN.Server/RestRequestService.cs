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

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        //isteği oluştur (body ve headerlarla beraber)
        using var httpRequest = BuildHttpRequest(dto);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var httpResponse = await client.SendAsync(httpRequest);
            stopwatch.Stop();

            requestLog.Response = await ParseSuccessResponseAsync(httpResponse, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            requestLog.Response = ParseErrorResponse(ex, stopwatch.ElapsedMilliseconds);
        }

        //db ye kayıt
        await _requestLogRepository.AddAsync(requestLog);
        return _mapper.Map<RequestLogDto>(requestLog);
    }


    private HttpRequestMessage BuildHttpRequest(CreateRequestLogDto dto)
    {
        var request = new HttpRequestMessage(new HttpMethod(dto.Method.ToString()), dto.Url);

        // Body ekleme işlemi
        if (!string.IsNullOrWhiteSpace(dto.Body) && dto.Method.ToString() != "GET")
        {
            request.Content = CreateHttpContent(dto);
        }

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
}