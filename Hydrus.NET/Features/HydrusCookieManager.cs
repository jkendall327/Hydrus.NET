using System.Text.Json;

namespace Hydrus.NET;

public record HydrusCookie(
    string Name,
    string Value,
    string Domain,
    string Path,
    double? Expires = null);

public record HydrusCookiesResponse(
    [property: JsonPropertyName("cookies")] List<HydrusCookie> Cookies);

internal class CookieJsonConverter : JsonConverter<HydrusCookie>
{
    public static readonly CookieJsonConverter Instance = new();
    public override HydrusCookie Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of array");

        reader.Read();
        var name = reader.GetString();
        reader.Read();
        var value = reader.GetString();
        reader.Read();
        var domain = reader.GetString();
        reader.Read();
        var path = reader.GetString();
        reader.Read();
        var expiration = reader.GetDouble();
        reader.Read(); // consume EndArray

        return new(name, value, domain, path, expiration);
    }

    public override void Write(Utf8JsonWriter writer, HydrusCookie cookie, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteStringValue(cookie.Name);
        writer.WriteStringValue(cookie.Value);
        writer.WriteStringValue(cookie.Domain);
        writer.WriteStringValue(cookie.Path);
        
        if (cookie.Expires.HasValue)
        {
            writer.WriteNumberValue(cookie.Expires.Value);
        }
        
        writer.WriteEndArray();
    }
}

public sealed class HydrusCookieManager(HttpClient httpClient)
{
    /// <summary>
    /// Get all cookies for a specific domain or all domains.
    /// </summary>
    /// <param name="domain">Domain to get cookies for.</param>
    public async Task<HydrusCookiesResponse> GetCookiesAsync(string domain)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);
        
        var url = $"manage_cookies/get_cookies?domain={Uri.EscapeDataString(domain)}";

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();
        
        return await response.ReadFromHydrusJsonAsync<HydrusCookiesResponse>(new()
        {
            Converters = { CookieJsonConverter.Instance }
        });
    }

    /// <summary>
    /// Set cookies for a specific domain.
    /// </summary>
    /// <param name="cookies">List of cookies to set</param>
    public async Task SetCookiesAsync(IEnumerable<HydrusCookie> cookies)
    {
        var response = await httpClient.PostAsJsonAsync("manage_cookies/set_cookies", new
        {
            cookies
        });
        
        response.EnsureSuccessStatusCode();
    }
}