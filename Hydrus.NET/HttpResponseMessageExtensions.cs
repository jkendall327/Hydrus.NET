namespace Hydrus.NET;

using System.Net.Http.Json;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> ReadFromHydrusJsonAsync<T>(this HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<T>();
        
        if (result is null)
        {
            throw new HydrusJsonDeserializationException(typeof(T));
        }
        
        return result;
    }
}