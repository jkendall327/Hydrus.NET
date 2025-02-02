namespace Hydrus.NET;

public record HydrusCookie(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("domain")] string Domain,
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("expires")] double? Expires = null);

public record HydrusCookiesResponse(
    [property: JsonPropertyName("cookies")] Dictionary<string, HydrusCookie> Cookies);

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
        
        return await response.ReadFromHydrusJsonAsync<HydrusCookiesResponse>();
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

    /// <summary>
    /// Remove cookies that match the given parameters.
    /// </summary>
    /// <param name="name">Optional cookie name to match</param>
    /// <param name="domain">Optional domain to match</param>
    /// <param name="path">Optional path to match</param>
    public async Task DeleteCookiesAsync(string? name = null, string? domain = null, string? path = null)
    {
        var requestContent = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(name))
            requestContent["name"] = name;
        if (!string.IsNullOrEmpty(domain))
            requestContent["domain"] = domain;
        if (!string.IsNullOrEmpty(path))
            requestContent["path"] = path;

        var response = await httpClient.PostAsJsonAsync("manage_cookies/delete_cookies", requestContent);
        response.EnsureSuccessStatusCode();
    }
}