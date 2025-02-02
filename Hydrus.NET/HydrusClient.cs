namespace Hydrus.NET;

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

    public HydrusClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public HydrusClient() : this(new HttpClient())
    {
        
    }
}