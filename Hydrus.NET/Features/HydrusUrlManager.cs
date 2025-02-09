using Microsoft.AspNetCore.WebUtilities;

namespace Hydrus.NET;

public record HydrusUrlResponse(
    [property: JsonPropertyName("normalised_url")] string NormalisedUrl,
    [property: JsonPropertyName("url_file_statuses")] HydrusUrlFileStatus[] UrlFileStatuses);

public record HydrusUrlFileStatus(
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("hash")] string Hash,
    [property: JsonPropertyName("note")] string Note);

public record HydrusUrlInfo(
    [property: JsonPropertyName("request_url")] string RequestUrl,
    [property: JsonPropertyName("normalised_url")] string NormalisedUrl,
    [property: JsonPropertyName("url_type")] UrlTypes UrlType,
    [property: JsonPropertyName("url_type_string")] string UrlTypeString,
    [property: JsonPropertyName("match_name")] string MatchName,
    [property: JsonPropertyName("can_parse")] bool CanParse);

public record HydrusUrlAddResponse(
    [property: JsonPropertyName("human_result_text")] string HumanResultText,
    [property: JsonPropertyName("normalised_url")] string NormalisedUrl);

public enum UrlTypes
{
    Post = 0,
    File = 2,
    Gallery = 3,
    Watchable = 4,
    Unknown = 5,
}

public sealed class HydrusUrlManager(HttpClient httpClient)
{
    /// <summary>
    /// Ask the client about an URL's files.
    /// </summary>
    /// <param name="url">The URL to query about</param>
    /// <param name="doublecheckFileSystem">Whether to verify 'already in db' results against the actual file system</param>
    public async Task<HydrusUrlResponse> GetUrlFilesAsync(string url, bool doublecheckFileSystem = false)
    {
        var uri = "add_urls/get_url_files";
        
        var args = new Dictionary<string, string?>
        {
            {
                "url", url
            },
            {
                "doublecheck_file_system", doublecheckFileSystem.ToString().ToLower()
            }
        };
        
        var query = QueryHelpers.AddQueryString(uri, args);
        
        var response = await httpClient.GetAsync(query);
        
        return await response.ReadFromHydrusJsonAsync<HydrusUrlResponse>();
    }

    /// <summary>
    /// Ask the client for information about a URL.
    /// </summary>
    /// <param name="url">The URL to get information about</param>
    public async Task<HydrusUrlInfo> GetUrlInfoAsync(string url)
    {
        var requestUrl = QueryHelpers.AddQueryString("add_urls/get_url_info", "url", url);

        var response = await httpClient.GetAsync(requestUrl);
        
        return await response.ReadFromHydrusJsonAsync<HydrusUrlInfo>();
    }

    /// <summary>
    /// Tell the client to 'import' a URL. This triggers the exact same routine as drag-and-dropping a text URL onto the main client window.
    /// </summary>
    /// <param name="url">The URL to add</param>
    /// <param name="destinationPageKey">Optional page identifier for the page to receive the URL</param>
    /// <param name="destinationPageName">Optional page name to receive the URL</param>
    /// <param name="fileServiceKey">Optional file service key to set where to import the file</param>
    /// <param name="showDestinationPage">Whether the UI should change pages on add</param>
    /// <param name="serviceKeysToAdditionalTags">Optional tags to give to any files imported from this URL</param>
    /// <param name="filterableTags">Optional tags to be filtered by any tag import options that applies to the URL</param>
    public async Task<HydrusUrlAddResponse> AddUrlAsync(
        string url,
        string? destinationPageKey = null,
        string? destinationPageName = null,
        string? fileServiceKey = null,
        bool? showDestinationPage = null,
        Dictionary<string, IEnumerable<string>>? serviceKeysToAdditionalTags = null,
        IEnumerable<string>? filterableTags = null)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["url"] = url
        };

        if (destinationPageKey != null)
            requestContent["destination_page_key"] = destinationPageKey;
            
        if (destinationPageName != null)
            requestContent["destination_page_name"] = destinationPageName;
            
        if (fileServiceKey != null)
            requestContent["file_service_key"] = fileServiceKey;
            
        if (showDestinationPage.HasValue)
            requestContent["show_destination_page"] = showDestinationPage.Value;
            
        if (serviceKeysToAdditionalTags != null)
            requestContent["service_keys_to_additional_tags"] = serviceKeysToAdditionalTags;
            
        if (filterableTags != null)
            requestContent["filterable_tags"] = filterableTags;

        var response = await httpClient.PostAsJsonAsync("add_urls/add_url", requestContent);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusUrlAddResponse>();
    }
}