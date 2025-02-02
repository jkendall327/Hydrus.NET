namespace Hydrus.NET;

public class HydrusClientManager(HttpClient httpClient)
{
    private HttpClient _httpClient = httpClient;
}