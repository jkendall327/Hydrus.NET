namespace Hydrus.NET;

/// <summary>
/// Options for configuring access to a Hydrus API instance.
/// </summary>
public class HydrusOptions
{
    /// <summary>
    /// The base URL the API is served from.
    /// </summary>
    public required string BaseUrl { get; set; }
    
    /// <summary>
    /// The access key you will use to authenticate with the client.
    /// </summary>
    public required string AccessKey { get; set; }
}