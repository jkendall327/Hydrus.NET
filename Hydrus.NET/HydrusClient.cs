namespace Hydrus.NET;

/// <summary>
/// Encapsulates HTTP requests to the Hydrus Client REST API.
/// </summary>
public class HydrusClient
{
    private readonly HttpClient _httpClient;
    
    public HydrusClientManager Client { get; private set; }
    public HydrusSearchManager Searches { get; private set; }
    public HydrusFileManager Files { get; private set; }
    public HydrusCookieManager Cookies { get; private set; }
    public HydrusHeaderManager Headers { get; private set; }
    public HydrusNoteManager Notes { get; private set; }
    public HydrusPageManager Pages { get; private set; }
    public HydrusPopupManager Popups { get; private set; }
    public HydrusRatingManager Ratings { get; private set; }
    public HydrusRelationshipManager Relationships { get; private set; }
    public HydrusServiceManager Services { get; private set; }
    public HydrusTagManager Tags { get; private set; }
    public HydrusTimeManager Times { get; private set; }
    public HydrusUrlManager Urls { get; private set; }

    /// <summary>
    /// Instantiates a new <see cref="HydrusClient"/> with a preconfigured <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">The client to use for HTTP calls to the Hydrus Client API.</param>
    public HydrusClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        Client = new(_httpClient);
        Searches = new(_httpClient);
        Files = new(httpClient);
        Cookies = new(_httpClient);
        Headers = new(_httpClient);
        Notes = new(_httpClient);
        Pages = new(_httpClient);
        Popups = new(_httpClient);
        Ratings = new(_httpClient);
        Relationships = new(_httpClient);
        Services = new(_httpClient);
        Tags = new(_httpClient);
        Times = new(_httpClient);
        Urls = new(_httpClient);
    }

    /// <summary>
    /// Instantiates a new <see cref="HydrusClient"/> from a base address and an access key.
    /// </summary>
    /// <param name="baseAddress">The base address the Hydrus API is served from.</param>
    /// <param name="accessKey">The access key used for API calls.</param>
    public HydrusClient(string baseAddress, string accessKey) : this(new()
    {
        BaseAddress = new(baseAddress),
        DefaultRequestHeaders = { { "Hydrus-Client-API-Access-Key", accessKey } }
    })
    {
        
    }
}