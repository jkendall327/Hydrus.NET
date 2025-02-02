namespace Hydrus.NET;

public record HydrusPendingCounts(
    [property: JsonPropertyName("pending_files")] int? PendingFiles,
    [property: JsonPropertyName("petitioned_files")] int? PetitionedFiles,
    [property: JsonPropertyName("pending_tag_mappings")] int? PendingTagMappings,
    [property: JsonPropertyName("petitioned_tag_mappings")] int? PetitionedTagMappings,
    [property: JsonPropertyName("pending_tag_siblings")] int? PendingTagSiblings,
    [property: JsonPropertyName("petitioned_tag_siblings")] int? PetitionedTagSiblings,
    [property: JsonPropertyName("pending_tag_parents")] int? PendingTagParents,
    [property: JsonPropertyName("petitioned_tag_parents")] int? PetitionedTagParents);

public record HydrusPendingCountsResponse(
    [property: JsonPropertyName("services")] Dictionary<string, HydrusService> Services,
    [property: JsonPropertyName("pending_counts")] Dictionary<string, HydrusPendingCounts> PendingCounts);

public sealed class HydrusServiceManager(HttpClient httpClient)
{
    /// <summary>
    /// Gets the counts of pending content for each upload-capable service.
    /// </summary>
    /// <returns>A dictionary of service keys to their pending content counts.</returns>
    public async Task<HydrusPendingCountsResponse> GetPendingCountsAsync()
    {
        var response = await httpClient.GetAsync("manage_services/get_pending_counts");
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPendingCountsResponse>();
    }

    /// <summary>
    /// Start the job to upload a service's pending content.
    /// </summary>
    /// <param name="serviceKey">The service key of the service to commit.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CommitPendingAsync(string serviceKey)
    {
        var requestContent = new Dictionary<string, string>
        {
            ["service_key"] = serviceKey
        };

        var response = await httpClient.PostAsJsonAsync("manage_services/commit_pending", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Forget all pending content for a service.
    /// </summary>
    /// <param name="serviceKey">The service key of the service to forget content for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ForgetPendingAsync(string serviceKey)
    {
        var requestContent = new Dictionary<string, string>
        {
            ["service_key"] = serviceKey
        };

        var response = await httpClient.PostAsJsonAsync("manage_services/forget_pending", requestContent);
        response.EnsureSuccessStatusCode();
    }
}