using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

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
    
    internal static async Task<TResponse> GetFromHydrusAsync<TResponse>(this HttpClient client,
        string url,
        CancellationToken token)
    {
        var response = await client.GetAsync(url, cancellationToken: token);

        response.EnsureSuccessStatusCode();

        return await ReadFromHydrusJsonAsync<TResponse>(response, token: token);
    }
    
    internal static async Task<TResponse> GetFromHydrusAsync<TResponse>(this HttpClient client,
        string rootUrl,
        Dictionary<string, object> parameters,
        CancellationToken token)
    {
        // TODO: this should probably use HttpUtility.UrlEncode or whatever instead of serialization.
        var transformed = parameters
            .ToDictionary(s => s.Key, string? (s) => JsonSerializer.Serialize(s.Value));

        var url = QueryHelpers.AddQueryString(rootUrl, transformed);
        
        var response = await client.GetAsync(url, cancellationToken: token);

        response.EnsureSuccessStatusCode();

        return await ReadFromHydrusJsonAsync<TResponse>(response, token: token);
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