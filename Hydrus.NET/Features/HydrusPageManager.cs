namespace Hydrus.NET;

public record HydrusPageKey(
    [property: JsonPropertyName("page_key")] string Key);

public record HydrusPageName(
    [property: JsonPropertyName("name")] string Name);

public record HydrusPage(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("page_key")] string PageKey,
    [property: JsonPropertyName("page_type")] int PageType,
    [property: JsonPropertyName("selected")] bool Selected);

public record HydrusPagesResponse(
    [property: JsonPropertyName("pages")] List<HydrusPage> Pages);

public record HydrusPageOfFilesRequest(
    [property: JsonPropertyName("page_key")] string PageKey,
    [property: JsonPropertyName("file_ids")] int[] FileIds);

public record HydrusPageOfImagesRequest(
    [property: JsonPropertyName("page_key")] string PageKey,
    [property: JsonPropertyName("hashes")] string[] Hashes);

public sealed class HydrusPageManager(HttpClient httpClient)
{
    /// <summary>
    /// Gets a list of all pages in the client.
    /// </summary>
    public async Task<HydrusPagesResponse> GetPagesAsync()
    {
        var response = await httpClient.GetAsync("manage_pages/get_pages");
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPagesResponse>();
    }

    /// <summary>
    /// Gets the name of a specific page.
    /// </summary>
    /// <param name="pageKey">The key of the page to get the name of</param>
    public async Task<HydrusPageName> GetPageNameAsync(string pageKey)
    {
        var response = await httpClient.GetAsync($"manage_pages/get_page_name?page_key={Uri.EscapeDataString(pageKey)}");
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPageName>();
    }

    /// <summary>
    /// Focus a specific page.
    /// </summary>
    /// <param name="pageKey">The key of the page to focus</param>
    public async Task FocusPageAsync(string pageKey)
    {
        var response = await httpClient.PostAsync($"manage_pages/focus_page?page_key={Uri.EscapeDataString(pageKey)}", null);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Add a new page of a specific type.
    /// </summary>
    /// <param name="pageType">The type of page to create</param>
    /// <param name="name">Optional name for the new page</param>
    /// <param name="syncToNewFiles">Optional parameter to sync the page to new files</param>
    /// <returns>The key of the newly created page</returns>
    public async Task<HydrusPageKey> AddPageAsync(int pageType, string? name = null, bool? syncToNewFiles = null)
    {
        var queryParams = new List<string> { $"page_type={pageType}" };
        
        if (!string.IsNullOrEmpty(name))
        {
            queryParams.Add($"name={Uri.EscapeDataString(name)}");
        }
        
        if (syncToNewFiles.HasValue)
        {
            queryParams.Add($"sync_to_new_files={syncToNewFiles.Value.ToString().ToLower()}");
        }

        var url = $"manage_pages/add_page?{string.Join("&", queryParams)}";
        var response = await httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPageKey>();
    }

    /// <summary>
    /// Modify a page's name.
    /// </summary>
    /// <param name="pageKey">The key of the page to rename</param>
    /// <param name="name">The new name for the page</param>
    public async Task ModifyPageNameAsync(string pageKey, string name)
    {
        var url = $"manage_pages/modify_page_name?page_key={Uri.EscapeDataString(pageKey)}&name={Uri.EscapeDataString(name)}";
        var response = await httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Add files to a page using their file IDs.
    /// </summary>
    /// <param name="pageKey">The key of the page to add files to</param>
    /// <param name="fileIds">Array of file IDs to add</param>
    public async Task AddFilesToPageAsync(string pageKey, int[] fileIds)
    {
        var request = new HydrusPageOfFilesRequest(pageKey, fileIds);
        var response = await httpClient.PostAsJsonAsync("manage_pages/add_files", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Add files to a page using their hashes.
    /// </summary>
    /// <param name="pageKey">The key of the page to add files to</param>
    /// <param name="hashes">Array of file hashes to add</param>
    public async Task AddFilesToPageAsync(string pageKey, string[] hashes)
    {
        var request = new HydrusPageOfImagesRequest(pageKey, hashes);
        var response = await httpClient.PostAsJsonAsync("manage_pages/add_files", request);
        response.EnsureSuccessStatusCode();
    }

    public static class PageTypes
    {
        public const int GalleryDownloader = 1;
        public const int SimpleDownloader = 2;
        public const int HardDriveImporter = 3;
        public const int PetitionPage = 4;
        public const int FileSearch = 5;
        public const int URLDownloader = 6;
        public const int Duplicates = 7;
        public const int ThreadWatcher = 8;
        public const int PageOfPages = 9;
    }
}