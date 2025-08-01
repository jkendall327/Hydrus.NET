namespace Hydrus.NET;

public class HydrusError
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }

    [JsonPropertyName("exception_type")]
    public required string ExceptionType { get; set; }

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
    
    [JsonPropertyName("version")]
    public int Version { get; set; }
    
    [JsonPropertyName("hydrus_version")]
    public int HydrusVersion { get; set; }
}