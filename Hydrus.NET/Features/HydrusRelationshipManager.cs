namespace Hydrus.NET;

public record HydrusRelationshipStatus(
    [property: JsonPropertyName("is_king")] bool IsKing,
    [property: JsonPropertyName("king")] string King,
    [property: JsonPropertyName("king_is_on_file_domain")] bool KingIsOnFileDomain,
    [property: JsonPropertyName("king_is_local")] bool KingIsLocal,
    [property: JsonPropertyName("0")] string[] PotentialDuplicates,
    [property: JsonPropertyName("1")] string[] FalsePositives,
    [property: JsonPropertyName("3")] string[] Alternates,
    [property: JsonPropertyName("8")] string[] Duplicates);

public record HydrusFileRelationshipsResponse(
    [property: JsonPropertyName("file_relationships")] Dictionary<string, HydrusRelationshipStatus> FileRelationships);

public record HydruspotentialDuplicatesCountResponse(
    [property: JsonPropertyName("potential_duplicates_count")] int PotentialDuplicatesCount);

public record HydrusPotentialDuplicatePairsResponse(
    [property: JsonPropertyName("potential_duplicate_pairs")] string[][] PotentialDuplicatePairs);

public record HydrusRandomPotentialDuplicatesResponse(
    [property: JsonPropertyName("random_potential_duplicate_hashes")] string[] RandomPotentialDuplicateHashes);

public record HydrusSetFileRelationship(
    [property: JsonPropertyName("hash_a")] string HashA,
    [property: JsonPropertyName("hash_b")] string HashB,
    [property: JsonPropertyName("relationship")] int Relationship,
    [property: JsonPropertyName("do_default_content_merge")] bool DoDefaultContentMerge,
    [property: JsonPropertyName("delete_a")] bool? DeleteA = null,
    [property: JsonPropertyName("delete_b")] bool? DeleteB = null);

public sealed class HydrusRelationshipManager
{
    private readonly HttpClient _httpClient;

    public static class RelationshipType
    {
        public const int SetAsPotentialDuplicates = 0;
        public const int SetAsFalsePositives = 1;
        public const int SetAsSameQuality = 2;
        public const int SetAsAlternates = 3;
        public const int SetABetter = 4;
        public const int SetBBetter = 7;
    }

    public static class PotentialsSearchType
    {
        public const int OneFileMatchesSearch1 = 0;
        public const int BothFilesMatchSearch1 = 1;
        public const int OneMatchesSearch1OtherMatchesSearch2 = 2;
    }

    public static class PixelDuplicatesType
    {
        public const int MustBePixelDuplicates = 0;
        public const int CanBePixelDuplicates = 1;
        public const int MustNotBePixelDuplicates = 2;
    }

    public HydrusRelationshipManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get the current relationships for one or more files.
    /// </summary>
    /// <param name="files">File identifiers to get relationships for</param>
    /// <param name="fileServiceKey">Optional file domain to limit results to</param>
    public async Task<HydrusFileRelationshipsResponse> GetFileRelationshipsAsync(HydrusFiles files,
        string? fileServiceKey = null)
    {
        var queryParams = new List<string>();

        if (files.FileId.HasValue)
            queryParams.Add($"file_id={files.FileId.Value}");
        if (files.FileIds != null)
            queryParams.Add(
                $"file_ids={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(files.FileIds))}");
        if (files.Hash != null)
            queryParams.Add($"hash={Uri.EscapeDataString(files.Hash)}");
        if (files.Hashes != null)
            queryParams.Add($"hashes={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(files.Hashes))}");
        if (fileServiceKey != null)
            queryParams.Add($"file_service_key={Uri.EscapeDataString(fileServiceKey)}");

        var url = $"manage_file_relationships/get_file_relationships?{string.Join("&", queryParams)}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusFileRelationshipsResponse>();
    }

    /// <summary>
    /// Get the count of remaining potential duplicate pairs in a particular search domain.
    /// </summary>
    /// <param name="fileServiceKey">Optional file domain</param>
    /// <param name="tagServiceKey1">Optional tag domain for first search</param>
    /// <param name="tags1">Optional tags for first search</param>
    /// <param name="tagServiceKey2">Optional tag domain for second search</param>
    /// <param name="tags2">Optional tags for second search</param>
    /// <param name="potentialsSearchType">How the pairs should match the searches</param>
    /// <param name="pixelDuplicates">Whether pairs should be pixel duplicates</param>
    /// <param name="maxHammingDistance">Maximum search distance</param>
    public async Task<HydruspotentialDuplicatesCountResponse> GetPotentialsCountAsync(string? fileServiceKey = null,
        string? tagServiceKey1 = null,
        IEnumerable<string>? tags1 = null,
        string? tagServiceKey2 = null,
        IEnumerable<string>? tags2 = null,
        int potentialsSearchType = PotentialsSearchType.OneFileMatchesSearch1,
        int pixelDuplicates = PixelDuplicatesType.CanBePixelDuplicates,
        int maxHammingDistance = 4)
    {
        var queryParams = new List<string>();

        if (fileServiceKey != null)
            queryParams.Add($"file_service_key={Uri.EscapeDataString(fileServiceKey)}");
        if (tagServiceKey1 != null)
            queryParams.Add($"tag_service_key_1={Uri.EscapeDataString(tagServiceKey1)}");
        if (tags1 != null)
            queryParams.Add($"tags_1={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(tags1))}");
        if (tagServiceKey2 != null)
            queryParams.Add($"tag_service_key_2={Uri.EscapeDataString(tagServiceKey2)}");
        if (tags2 != null)
            queryParams.Add($"tags_2={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(tags2))}");

        queryParams.Add($"potentials_search_type={potentialsSearchType}");
        queryParams.Add($"pixel_duplicates={pixelDuplicates}");
        queryParams.Add($"max_hamming_distance={maxHammingDistance}");

        var url = $"manage_file_relationships/get_potentials_count?{string.Join("&", queryParams)}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydruspotentialDuplicatesCountResponse>();
    }

    /// <summary>
    /// Get some potential duplicate pairs for a filtering workflow. 
    /// </summary>
    /// <param name="fileServiceKey">Optional file domain</param>
    /// <param name="tagServiceKey1">Optional tag domain for first search</param>
    /// <param name="tags1">Optional tags for first search</param>
    /// <param name="tagServiceKey2">Optional tag domain for second search</param>
    /// <param name="tags2">Optional tags for second search</param>
    /// <param name="potentialsSearchType">How the pairs should match the searches</param>
    /// <param name="pixelDuplicates">Whether pairs should be pixel duplicates</param>
    /// <param name="maxHammingDistance">Maximum search distance</param>
    /// <param name="maxNumPairs">Maximum number of pairs to return</param>
    public async Task<HydrusPotentialDuplicatePairsResponse> GetPotentialPairsAsync(string? fileServiceKey = null,
        string? tagServiceKey1 = null,
        IEnumerable<string>? tags1 = null,
        string? tagServiceKey2 = null,
        IEnumerable<string>? tags2 = null,
        int potentialsSearchType = PotentialsSearchType.OneFileMatchesSearch1,
        int pixelDuplicates = PixelDuplicatesType.CanBePixelDuplicates,
        int maxHammingDistance = 4,
        int? maxNumPairs = null)
    {
        var queryParams = new List<string>();

        if (fileServiceKey != null)
            queryParams.Add($"file_service_key={Uri.EscapeDataString(fileServiceKey)}");
        if (tagServiceKey1 != null)
            queryParams.Add($"tag_service_key_1={Uri.EscapeDataString(tagServiceKey1)}");
        if (tags1 != null)
            queryParams.Add($"tags_1={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(tags1))}");
        if (tagServiceKey2 != null)
            queryParams.Add($"tag_service_key_2={Uri.EscapeDataString(tagServiceKey2)}");
        if (tags2 != null)
            queryParams.Add($"tags_2={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(tags2))}");
        if (maxNumPairs.HasValue)
            queryParams.Add($"max_num_pairs={maxNumPairs.Value}");

        queryParams.Add($"potentials_search_type={potentialsSearchType}");
        queryParams.Add($"pixel_duplicates={pixelDuplicates}");
        queryParams.Add($"max_hamming_distance={maxHammingDistance}");

        var url = $"manage_file_relationships/get_potential_pairs?{string.Join("&", queryParams)}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPotentialDuplicatePairsResponse>();
    }

    /// <summary>
    /// Get some random potentially duplicate file hashes.
    /// </summary>
    /// <param name="fileServiceKey">Optional file domain</param>
    /// <param name="tagServiceKey1">Optional tag domain for first search</param>
    /// <param name="tags1">Optional tags for first search</param>
    /// <param name="tagServiceKey2">Optional tag domain for second search</param>
    /// <param name="tags2">Optional tags for second search</param>
    /// <param name="potentialsSearchType">How the files should match the searches</param>
    /// <param name="pixelDuplicates">Whether files should be pixel duplicates</param>
    /// <param name="maxHammingDistance">Maximum search distance</param>
    public async Task<HydrusRandomPotentialDuplicatesResponse> GetRandomPotentialsAsync(string? fileServiceKey = null,
        string? tagServiceKey1 = null,
        IEnumerable<string>? tags1 = null,
        string? tagServiceKey2 = null,
        IEnumerable<string>? tags2 = null,
        int potentialsSearchType = PotentialsSearchType.OneFileMatchesSearch1,
        int pixelDuplicates = PixelDuplicatesType.CanBePixelDuplicates,
        int maxHammingDistance = 4)
    {
        var queryParams = new List<string>();

        if (fileServiceKey != null)
            queryParams.Add($"file_service_key={Uri.EscapeDataString(fileServiceKey)}");
        if (tagServiceKey1 != null)
            queryParams.Add($"tag_service_key_1={Uri.EscapeDataString(tagServiceKey1)}");
        if (tags1 != null)
            queryParams.Add($"tags_1={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(tags1))}");
        if (tagServiceKey2 != null)
            queryParams.Add($"tag_service_key_2={Uri.EscapeDataString(tagServiceKey2)}");
        if (tags2 != null)
            queryParams.Add($"tags_2={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(tags2))}");

        queryParams.Add($"potentials_search_type={potentialsSearchType}");
        queryParams.Add($"pixel_duplicates={pixelDuplicates}");
        queryParams.Add($"max_hamming_distance={maxHammingDistance}");

        var url = $"manage_file_relationships/get_random_potentials?{string.Join("&", queryParams)}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusRandomPotentialDuplicatesResponse>();
    }

    /// <summary>
    /// Remove all potential pairs that any of the given files are a part of.
    /// </summary>
    /// <param name="files">File identifiers to remove potentials for</param>
    public async Task RemovePotentialsAsync(HydrusFiles files)
    {
        var requestContent = new Dictionary<string, object>();

        if (files.FileId.HasValue)
            requestContent["file_id"] = files.FileId.Value;
        if (files.FileIds != null)
            requestContent["file_ids"] = files.FileIds;
        if (files.Hash != null)
            requestContent["hash"] = files.Hash;
        if (files.Hashes != null)
            requestContent["hashes"] = files.Hashes;

        var response = await _httpClient.PostAsJsonAsync("manage_file_relationships/remove_potentials", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Set the relationships between specified file pairs.
    /// </summary>
    /// <param name="relationships">List of relationships to set</param>
    public async Task SetFileRelationshipsAsync(IEnumerable<HydrusSetFileRelationship> relationships)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["relationships"] = relationships
        };

        var response = await _httpClient
            .PostAsJsonAsync("manage_file_relationships/set_file_relationships", requestContent);
        
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Sets the specified files to be the kings of their duplicate groups.
    /// </summary>
    /// <param name="files">File identifiers to promote to kings</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Files will be promoted to kings of their duplicate groups in the order provided.
    /// If a file is already king (also true for files with no duplicates), this is idempotent.
    /// If multiple files from the same group are specified, the latter will be king at the end.
    /// </remarks>
    public async Task SetKingsAsync(HydrusFiles files)
    {
        var requestContent = new Dictionary<string, object>();

        if (files.FileId.HasValue)
            requestContent["file_id"] = files.FileId.Value;
        if (files.FileIds != null)
            requestContent["file_ids"] = files.FileIds;
        if (files.Hash != null)
            requestContent["hash"] = files.Hash;
        if (files.Hashes != null)
            requestContent["hashes"] = files.Hashes;

        var response = await _httpClient.PostAsJsonAsync("manage_file_relationships/set_kings", requestContent);
        response.EnsureSuccessStatusCode();
    }
}