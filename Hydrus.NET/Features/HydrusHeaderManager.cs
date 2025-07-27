namespace Hydrus.NET;

public record HydrusHeaderValue(
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("approved")]
    string? Approved = null,
    [property: JsonPropertyName("reason")] string? Reason = null);

public record HydrusHeadersResponse(
    [property: JsonPropertyName("network_context")]
    HydrusNetworkContext? NetworkContext,
    [property: JsonPropertyName("headers")]
    Dictionary<string, HydrusHeaderValue> Headers);

public record HydrusNetworkContext(
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("data")] string Data);

public record HydrusSetHeadersRequest(string? Domain, Dictionary<string, HydrusHeaderValue> Headers);

public record HydrusSetUserAgentRequest(
    [property: JsonPropertyName("user-agent")]
    string UserAgent);

public sealed class HydrusHeaderManager(HttpClient httpClient)
{
    /// <summary>
    /// Get the custom HTTP headers for a specific domain or globally.
    /// </summary>
    /// <param name="domain">Optional domain to get headers for. If not provided, returns global headers.</param>
    public async Task<HydrusHeadersResponse> GetHeadersAsync(string? domain = null)
    {
        var url = Constants.GET_HEADERS;

        if (!string.IsNullOrEmpty(domain))
        {
            url += $"?domain={Uri.EscapeDataString(domain)}";
        }

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.ReadFromHydrusJsonAsync<HydrusHeadersResponse>();
    }

    /// <summary>
    /// Set custom HTTP headers for a specific domain or globally.
    /// </summary>
    /// <param name="headers">Dictionary of header names to their values and metadata</param>
    /// <param name="domain">Optional domain to set headers for. If not provided, sets global headers.</param>
    public async Task SetHeadersAsync(Dictionary<string, HydrusHeaderValue> headers,
        string? domain = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HydrusSetHeadersRequest(domain, headers);
        await httpClient.PostToHydrusAsync(Constants.SET_HEADERS, request, cancellationToken);
    }

    /// <summary>
    /// Set the global User-Agent header. This method is deprecated - use SetHeaders instead.
    /// </summary>
    /// <param name="userAgent">The User-Agent string to set. Send empty string to reset to default.</param>
    [Obsolete("This method is deprecated. Use SetHeaders instead.")]
    public async Task SetUserAgentAsync(string userAgent, CancellationToken cancellationToken = default)
    {
        var request = new HydrusSetUserAgentRequest(userAgent);
        await httpClient.PostToHydrusAsync(Constants.SET_USER_AGENT, request, cancellationToken);
    }
}