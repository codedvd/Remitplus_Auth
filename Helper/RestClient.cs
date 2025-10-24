using Serilog;
using System.Net.Http.Headers;
using System.Text;

namespace Remitplus_Authentication.Helper
{
    public interface IRestClient
    {
        Task<(HttpResponseMessage Response, string Content)> MakeApiCallAsync(
            string url,
            HttpMethod method,
            Dictionary<string, string>? headers = null,
            string? rawBody = null,
            Dictionary<string, string>? formData = null,
            Dictionary<string, Stream>? fileData = null);
    }

    public class RestClient : IRestClient, IDisposable
    {
        private readonly HttpClient _httpClient;

        public RestClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(HttpResponseMessage Response, string Content)> MakeApiCallAsync(
            string url,
            HttpMethod method,
            Dictionary<string, string>? headers = null,
            string? rawBody = null,
            Dictionary<string, string>? formData = null,
            Dictionary<string, Stream>? fileData = null)
        {
            try
            {
                Log.Information("Initiating API call. Url: {Url}, Method: {Method}", url, method);

                var request = new HttpRequestMessage(method, url);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                int contentTypes = (rawBody != null ? 1 : 0) + (formData != null ? 1 : 0) + (fileData != null ? 1 : 0);
                if (contentTypes > 1)
                {
                    throw new ArgumentException("Cannot specify more than one of rawBody, formData, or fileData.");
                }

                if (rawBody != null)
                {
                    request.Content = new StringContent(rawBody, Encoding.UTF8);
                    if (!headers?.ContainsKey("Content-Type") ?? true)
                    {
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }
                }
                else if (formData != null)
                {
                    request.Content = new FormUrlEncodedContent(formData);
                }
                else if (fileData != null)
                {
                    var multipartContent = new MultipartFormDataContent();
                    foreach (var file in fileData)
                    {
                        var streamContent = new StreamContent(file.Value);
                        multipartContent.Add(streamContent, file.Key, file.Key);
                    }
                    request.Content = multipartContent;
                    if (!headers?.ContainsKey("Content-Type") ?? true)
                    {
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                    }
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                string content = await response.Content.ReadAsStringAsync();

                Log.Information("API call completed. Url: {Url}, StatusCode: {StatusCode}, Response: {Response}", url, response.StatusCode, content);

                return (response, content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "API call failed. Url: {Url}, Method: {Method}, Headers: {@Headers}, RawBody: {RawBody}, FormData: {@FormData}, FileData: {@FileData}",
                    url, method, headers, rawBody, formData, fileData?.Keys);
                throw new HttpRequestException($"API call failed: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            Log.Information("HttpClient disposed in RestClient.");
        }
    }
}
