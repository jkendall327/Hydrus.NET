using System.Text.Json;

namespace Hydrus.NET;

using System.Net.Http.Json;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> ReadFromHydrusJsonAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(options);
        
        if (result is null)
        {
            throw new HydrusJsonDeserializationException(typeof(T));
        }
        
        return result;
    }
}