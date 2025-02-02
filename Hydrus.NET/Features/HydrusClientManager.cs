namespace Hydrus.NET;

public record HydrusVersion(
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("client_version")] int ClientVersion);

public record HydrusAccessKey(
    [property: JsonPropertyName("access_key")] string AccessKey);

public record HydrusSessionKey(
    [property: JsonPropertyName("session_key")] string SessionKey);

public record HydrusService(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("service_key")] string ServiceKey,
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("type_pretty")] string TypePretty);

public record HydrusServiceResponse(
    [property: JsonPropertyName("service")] HydrusService Service);

public record HydrusServicesResponse(
    [property: JsonPropertyName("services")] Dictionary<string, HydrusService> Services);

public sealed class HydrusClientManager(HttpClient httpClient)
{
    /// <summary>
    /// Gets the current API version and hydrus client version.
    /// </summary>
    public async Task<HydrusVersion> GetVersionAsync()
    {
        var response = await httpClient.GetAsync("api_version");
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusVersion>();
    }

    /// <summary>
    /// Requests new API access permissions. This requires the 'add from api request' dialog to be open.
    /// </summary>
    /// <param name="name">Descriptive name of the access</param>
    /// <param name="permitsEverything">Whether to permit all tasks now and in future</param>
    /// <param name="basicPermissions">List of numerical permission identifiers to request</param>
    public async Task<HydrusAccessKey> RequestNewPermissionsAsync(
        string name,
        bool? permitsEverything = null,
        IEnumerable<int>? basicPermissions = null)
    {
        var queryParams = new List<string> { $"name={Uri.EscapeDataString(name)}" };
        
        if (permitsEverything.HasValue)
        {
            queryParams.Add($"permits_everything={permitsEverything.Value.ToString().ToLower()}");
        }

        if (basicPermissions != null)
        {
            var permissionsJson = System.Text.Json.JsonSerializer.Serialize(basicPermissions);
            queryParams.Add($"basic_permissions={Uri.EscapeDataString(permissionsJson)}");
        }

        var url = $"request_new_permissions?{string.Join("&", queryParams)}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusAccessKey>();
    }

    /// <summary>
    /// Gets a new session key that can be used in place of an access key.
    /// </summary>
    public async Task<HydrusSessionKey> GetSessionKeyAsync()
    {
        var response = await httpClient.GetAsync("session_key");
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusSessionKey>();
    }

    /// <summary>
    /// Verifies if an access key is valid.
    /// </summary>
    public async Task<bool> VerifyAccessKeyAsync()
    {
        var response = await httpClient.GetAsync("verify_access_key");
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Gets information about a specific service by name or key.
    /// </summary>
    /// <param name="serviceName">Name of the service to look up</param>
    /// <param name="serviceKey">Key of the service to look up</param>
    public async Task<HydrusServiceResponse> GetServiceAsync(string? serviceName = null, string? serviceKey = null)
    {
        if (string.IsNullOrEmpty(serviceName) && string.IsNullOrEmpty(serviceKey))
        {
            throw new ArgumentException("Either serviceName or serviceKey must be provided");
        }

        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(serviceName))
        {
            queryParams.Add($"service_name={Uri.EscapeDataString(serviceName)}");
        }
        if (!string.IsNullOrEmpty(serviceKey))
        {
            queryParams.Add($"service_key={Uri.EscapeDataString(serviceKey)}");
        }

        var url = $"get_service?{string.Join("&", queryParams)}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusServiceResponse>();
    }

    /// <summary>
    /// Gets information about all services in the client.
    /// </summary>
    public async Task<HydrusServicesResponse> GetServicesAsync()
    {
        var response = await httpClient.GetAsync("get_services");
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusServicesResponse>();
    }
}