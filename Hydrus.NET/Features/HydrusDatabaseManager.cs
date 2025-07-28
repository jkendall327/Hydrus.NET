namespace Hydrus.NET;

public class BonedStats
{
    [JsonPropertyName("num_inbox")]
    public int NumInbox { get; set; }

    [JsonPropertyName("num_archive")]
    public int NumArchive { get; set; }

    [JsonPropertyName("num_deleted")]
    public int NumDeleted { get; set; }

    [JsonPropertyName("size_inbox")]
    public long SizeInbox { get; set; }

    [JsonPropertyName("size_archive")]
    public long SizeArchive { get; set; }

    [JsonPropertyName("size_deleted")]
    public long SizeDeleted { get; set; }

    [JsonPropertyName("earliest_import_time")]
    public long EarliestImportTime { get; set; }

    [JsonPropertyName("total_viewtime")]
    public int[]? TotalViewtime { get; set; }

    [JsonPropertyName("total_alternate_files")]
    public int TotalAlternateFiles { get; set; }

    [JsonPropertyName("total_duplicate_files")]
    public int TotalDuplicateFiles { get; set; }

    [JsonPropertyName("total_potential_pairs")]
    public int TotalPotentialPairs { get; set; }
}

public class HydrusDatabaseManager(HttpClient client)
{
    public async Task ForceCommitAsync(CancellationToken cancellationToken = default)
    {
        await client.PostToHydrusAsync(Constants.FORCE_COMMIT, cancellationToken);
    }

    public async Task LockOnAsync(CancellationToken cancellationToken = default)
    {
        await client.PostToHydrusAsync(Constants.LOCK_ON, cancellationToken);
    }
    
    public async Task LockOffAsync(CancellationToken cancellationToken = default)
    {
        await client.PostToHydrusAsync(Constants.LOCK_OFF, cancellationToken);
    }

    public async Task<BonedStats> MrBonesAsync(
        IEnumerable<string>? tags = null,
        string? fileDomain = null,
        string? tagServiceKey = null,
        CancellationToken cancellationToken = default)
    {
        var req = new Dictionary<string, object>();
        
        var response = await client.GetFromHydrusAsync<BonedStats>(Constants.MR_BONES, req, cancellationToken);

        return response;
    }

    public async Task<string> GetClientOptionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(Constants.GET_CLIENT_OPTIONS, cancellationToken);

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}