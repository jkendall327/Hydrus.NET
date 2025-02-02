using System.Text.Json.Serialization;

namespace Hydrus.NET;

public record HydrusVersion(
    int Version, 
    int ClientVersion)
{
    [JsonPropertyName("version")]
    public int Version { get; init; } = Version;
    
    [JsonPropertyName("client_version")]
    public int ClientVersion { get; init; } = ClientVersion;
}

public class HydrusClientManager(HttpClient httpClient)
{
    private HttpClient _httpClient = httpClient;

    public async Task<HydrusVersion> GetHydrusVersionAsync()
    {
        throw new NotImplementedException();
    }
}