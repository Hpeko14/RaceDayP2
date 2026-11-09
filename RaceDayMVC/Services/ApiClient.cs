using System.Net.Http.Headers;
using System.Text.Json;

namespace RaceDayMVC.Services;

public class ApiResult
{
    public bool Success { get; set; }
    public int Status { get; set; }
    /// <summary>A friendly message that is safe to show to the user.</summary>
    public string Error { get; set; } = "";
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; set; }
}

/// <summary>
/// The only way this MVC app gets or changes data: it calls the Part 2 API.
/// The API's session cookie is kept in our own session and sent along with every request.
/// </summary>
public class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string ApiSessionCookieName = ".AspNetCore.Session=";

    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _accessor;

    public ApiClient(HttpClient http, IHttpContextAccessor accessor)
    {
        _http = http;
        _accessor = accessor;
    }

    public Task<ApiResult<T>> GetAsync<T>(string url) => SendAsync<T>(HttpMethod.Get, url, null);
    public Task<ApiResult<T>> PostAsync<T>(string url, object? body = null) => SendAsync<T>(HttpMethod.Post, url, ToJson(body));
    public Task<ApiResult<T>> PutAsync<T>(string url, object? body) => SendAsync<T>(HttpMethod.Put, url, ToJson(body));
    public Task<ApiResult> PostAsync(string url, object? body = null) => SendNoDataAsync(HttpMethod.Post, url, ToJson(body));
    public Task<ApiResult> PutAsync(string url, object? body) => SendNoDataAsync(HttpMethod.Put, url, ToJson(body));
    public Task<ApiResult> DeleteAsync(string url) => SendNoDataAsync(HttpMethod.Delete, url, null);

    /// <summary>Sends an uploaded file to the API (multipart field "file"). The API puts it in Azure Blob Storage.</summary>
    public Task<ApiResult<T>> UploadAsync<T>(string url, IFormFile file)
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
        content.Add(fileContent, "file", file.FileName);
        return SendAsync<T>(HttpMethod.Post, url, content);
    }

    private static HttpContent? ToJson(object? body) =>
        body is null ? null : JsonContent.Create(body, body.GetType(), options: JsonOptions);

    private async Task<ApiResult> SendNoDataAsync(HttpMethod method, string url, HttpContent? content)
    {
        var (result, _) = await ExecuteAsync(method, url, content);
        return result;
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, HttpContent? content)
    {
        var (basic, body) = await ExecuteAsync(method, url, content);
        var result = new ApiResult<T> { Success = basic.Success, Status = basic.Status, Error = basic.Error };
        if (basic.Success && !string.IsNullOrWhiteSpace(body))
        {
            try { result.Data = JsonSerializer.Deserialize<T>(body, JsonOptions); }
            catch (JsonException)
            {
                result.Success = false;
                result.Error = "The server sent a response we could not read. Please try again.";
            }
        }
        return result;
    }

    private async Task<(ApiResult Result, string Body)> ExecuteAsync(HttpMethod method, string url, HttpContent? content)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url) { Content = content };
            var cookie = _accessor.HttpContext?.Session.GetString(SessionKeys.ApiCookie);
            if (!string.IsNullOrEmpty(cookie)) request.Headers.TryAddWithoutValidation("Cookie", cookie);

            using var response = await _http.SendAsync(request);
            CaptureSessionCookie(response);

            var body = await response.Content.ReadAsStringAsync();
            var status = (int)response.StatusCode;
            var result = new ApiResult
            {
                Success = response.IsSuccessStatusCode,
                Status = status,
                Error = response.IsSuccessStatusCode ? "" : FriendlyError(body, status)
            };
            return (result, body);
        }
        catch (HttpRequestException)
        {
            return (Unavailable(), "");
        }
        catch (TaskCanceledException)
        {
            return (Unavailable(), "");
        }
    }

    private static ApiResult Unavailable() => new()
    {
        Success = false,
        Status = 503,
        Error = "RaceDay is not reachable right now. Please try again in a moment."
    };

    private void CaptureSessionCookie(HttpResponseMessage response)
    {
        var session = _accessor.HttpContext?.Session;
        if (session is null || !response.Headers.TryGetValues("Set-Cookie", out var cookies)) return;
        foreach (var header in cookies)
        {
            if (header.StartsWith(ApiSessionCookieName, StringComparison.Ordinal))
                session.SetString(SessionKeys.ApiCookie, header.Split(';')[0]);
        }
    }

    /// <summary>Turns the API's error body into a short message. Raw errors are never shown to users.</summary>
    private static string FriendlyError(string body, int status)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
                    return message.GetString() ?? "";

                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    var messages = new List<string>();
                    foreach (var field in errors.EnumerateObject())
                    {
                        if (field.Value.ValueKind != JsonValueKind.Array) continue;
                        foreach (var item in field.Value.EnumerateArray())
                            if (item.ValueKind == JsonValueKind.String) messages.Add(item.GetString() ?? "");
                    }
                    if (messages.Count > 0) return string.Join(" ", messages.Distinct());
                }
            }
        }
        catch (JsonException) { /* not JSON, fall through */ }

        return status switch
        {
            400 => "Some of the information entered is not valid.",
            401 => "Please log in again.",
            403 => "You do not have permission to do that.",
            404 => "We could not find what you were looking for.",
            409 => "That conflicts with existing information.",
            _ => "Something went wrong. Please try again."
        };
    }
}