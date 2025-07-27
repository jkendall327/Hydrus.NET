using System.Text.Json;

namespace Hydrus.NET;

internal static class HttpExtensions
{
    internal static async Task<HttpResponseMessage> PostToHydrusAsync<T>(this HttpClient client,
        string url,
        T content,
        CancellationToken token)
    {
        var response = await client.PostAsJsonAsync(url, content, cancellationToken: token);

        response.EnsureSuccessStatusCode();

        return response;
    }

    internal static async Task<T> ReadFromHydrusJsonAsync<T>(this HttpResponseMessage response,
        JsonSerializerOptions? options = null, CancellationToken token = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<HydrusError>(cancellationToken: token);

            if (error is null)
            {
                throw new HydrusJsonDeserializationException(typeof(HydrusError));
            }

            throw new HydrusException(error.ExceptionType, error.Error);
        }

        var result = await response.Content.ReadFromJsonAsync<T>(options, cancellationToken: token);

        if (result is null)
        {
            throw new HydrusJsonDeserializationException(typeof(T));
        }

        return result;
    }
}