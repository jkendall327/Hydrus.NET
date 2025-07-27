namespace Hydrus.NET;

public record HydrusTag(
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("action")] int Action // 0: Add, 1: Delete, etc.
);

public record HydrusTagChanges(
    [property: JsonPropertyName("tags_added")]
    Dictionary<string, List<string>> TagsAdded,
    [property: JsonPropertyName("tags_deleted")]
    Dictionary<string, List<string>> TagsDeleted);

public record HydrusFile(
    [property: JsonPropertyName("file_id")]
    int FileId,
    [property: JsonPropertyName("hash")] string Hash,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("mime")] string Mime,
    [property: JsonPropertyName("filetype_forced")]
    bool FiletypeForced,
    [property: JsonPropertyName("filetype_human")]
    string FiletypeHuman,
    [property: JsonPropertyName("ext")] string Ext,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("duration")]
    float? Duration,
    [property: JsonPropertyName("time_modified")]
    long? TimeModified,
    [property: JsonPropertyName("file_services")]
    FileServices FileServices,
    [property: JsonPropertyName("ipfs_multihashes")]
    Dictionary<string, string> IpfsMultihashes,
    [property: JsonPropertyName("has_audio")]
    bool HasAudio,
    [property: JsonPropertyName("blurhash")]
    string Blurhash,
    [property: JsonPropertyName("pixel_hash")]
    string PixelHash,
    [property: JsonPropertyName("num_frames")]
    int? NumFrames,
    [property: JsonPropertyName("num_words")]
    int? NumWords,
    [property: JsonPropertyName("is_inbox")]
    bool IsInbox,
    [property: JsonPropertyName("is_local")]
    bool IsLocal,
    [property: JsonPropertyName("is_trashed")]
    bool IsTrashed,
    [property: JsonPropertyName("is_deleted")]
    bool IsDeleted,
    [property: JsonPropertyName("has_exif")]
    bool HasExif,
    [property: JsonPropertyName("has_human_readable_embedded_metadata")]
    bool HasHumanReadableEmbeddedMetadata,
    [property: JsonPropertyName("has_icc_profile")]
    bool HasIccProfile,
    [property: JsonPropertyName("has_transparency")]
    bool HasTransparency,
    [property: JsonPropertyName("known_urls")]
    List<string> KnownUrls,
    [property: JsonPropertyName("ratings")]
    Dictionary<string, object> Ratings,
    [property: JsonPropertyName("tags")] Dictionary<string, TagInfo> Tags,
    [property: JsonPropertyName("file_viewing_statistics")]
    List<FileViewingStatistics> FileViewingStatistics);

public class FileServices
{
    [JsonPropertyName("current")] public required Dictionary<string, ServiceInfo> Current { get; set; }

    [JsonPropertyName("deleted")] public required Dictionary<string, ServiceInfo> Deleted { get; set; }
}

public class ServiceInfo
{
    [JsonPropertyName("time_imported")] public long? TimeImported { get; set; }

    [JsonPropertyName("time_deleted")] public long? TimeDeleted { get; set; }
}

public class TagInfo
{
    [JsonPropertyName("storage_tags")] public required Dictionary<string, List<string>> StorageTags { get; set; }

    [JsonPropertyName("display_tags")] public required Dictionary<string, List<string>> DisplayTags { get; set; }
}

public class FileViewingStatistics
{
    [JsonPropertyName("canvas_type")] public int CanvasType { get; set; }

    [JsonPropertyName("canvas_type_pretty")]
    public required string CanvasTypePretty { get; set; }

    [JsonPropertyName("views")] public int Views { get; set; }

    [JsonPropertyName("viewtime")] public float Viewtime { get; set; }

    [JsonPropertyName("last_viewed_timestamp")]
    public long? LastViewedTimestamp { get; set; }
}

public record HydrusFileRelationship(
    [property: JsonPropertyName("is_king")]
    bool IsKing,
    [property: JsonPropertyName("king")] string King,
    [property: JsonPropertyName("king_is_on_file_domain")]
    bool KingIsOnFileDomain,
    [property: JsonPropertyName("king_is_local")]
    bool KingIsLocal,
    [property: JsonPropertyName("0")] List<string> PotentialDuplicates,
    [property: JsonPropertyName("1")] List<string> FalsePositives,
    [property: JsonPropertyName("3")] List<string> Alternates,
    [property: JsonPropertyName("8")] List<string> Duplicates);

public sealed class HydrusFileManager(HttpClient client)
{
    /// <summary>
    /// Add or update tags associated with a file using its hash.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file in hexadecimal.</param>
    /// <param name="tags">Dictionary mapping service keys to lists of tags.</param>
    /// <param name="preventSpaceTags">Whether to block any tags intended for namespaces.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The tags that were actually added or deleted.</returns>
    public async Task<HydrusTagChanges> SetTagsAsync(string hash,
        Dictionary<string, List<string>> tags,
        bool? preventSpaceTags = null,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["tags"] = tags
        };

        if (preventSpaceTags.HasValue)
        {
            requestContent["prevent_space_tags"] = preventSpaceTags.Value;
        }

        var response = await client.PostAsJsonAsync("add_tags/add_tags", requestContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.ReadFromHydrusJsonAsync<HydrusTagChanges>(token: cancellationToken);
    }

    /// <summary>
    /// Add or update tags associated with a file using its file ID.
    /// </summary>
    /// <param name="fileId">The numerical ID of the file.</param>
    /// <param name="tags">Dictionary mapping service keys to lists of tags.</param>
    /// <param name="preventSpaceTags">Whether to block any tags intended for namespaces.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The tags that were actually added or deleted.</returns>
    public async Task<HydrusTagChanges> SetTagsAsync(int fileId,
        Dictionary<string, List<string>> tags,
        bool? preventSpaceTags = null,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["file_id"] = fileId,
            ["tags"] = tags
        };

        if (preventSpaceTags.HasValue)
        {
            requestContent["prevent_space_tags"] = preventSpaceTags.Value;
        }

        var response = await client.PostAsJsonAsync("add_tags/add_tags", requestContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.ReadFromHydrusJsonAsync<HydrusTagChanges>(token: cancellationToken);
    }

    /// <summary>
    /// Remove tags associated with a file using its hash.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file in hexadecimal.</param>
    /// <param name="serviceKeys">List of service keys for tags to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the delete operation.</returns>
    public async Task DeleteTagsAsync(string hash,
        IEnumerable<string> serviceKeys,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["service_keys"] = serviceKeys
        };

        await client.PostToHydrusAsync(Constants.DELETE_TAGS, requestContent, cancellationToken);
    }

    /// <summary>
    /// Remove tags associated with a file using its file ID.
    /// </summary>
    /// <param name="fileId">The numerical ID of the file.</param>
    /// <param name="serviceKeys">List of service keys for tags to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the delete operation.</returns>
    public async Task DeleteTagsAsync(int fileId,
        IEnumerable<string> serviceKeys,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["file_id"] = fileId,
            ["service_keys"] = serviceKeys
        };

        await client.PostToHydrusAsync(Constants.DELETE_TAGS, requestContent, cancellationToken);
    }

    /// <summary>
    /// Add a file using its path.
    /// </summary>
    /// <param name="path">The file path to import.</param>
    /// <param name="deleteAfterSuccess">Whether to delete the source file after successful import.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The imported file's information.</returns>
    public async Task<HydrusFile> AddFileAsync(string path,
        bool deleteAfterSuccess = false,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["path"] = path,
            ["delete_after_successful_import"] = deleteAfterSuccess
        };

        var response = await client.PostAsJsonAsync(Constants.ADD_FILE, requestContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.ReadFromHydrusJsonAsync<HydrusFile>(token: cancellationToken);
    }

    /// <summary>
    /// Delete a file using its hash.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file.</param>
    /// <param name="serviceKeys">List of service keys to delete from.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the delete operation.</returns>
    public async Task DeleteFileAsync(string hash,
        IEnumerable<string> serviceKeys,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["service_keys"] = serviceKeys
        };

        var response = await client.PostToHydrusAsync(Constants.DELETE_FILES, requestContent, cancellationToken);
    }

    /// <summary>
    /// Undelete a file using its hash.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file.</param>
    /// <param name="serviceKeys">List of service keys to undelete to.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the undelete operation.</returns>
    public async Task UndeleteFileAsync(string hash,
        IEnumerable<string> serviceKeys,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["service_keys"] = serviceKeys
        };

        var response = await client.PostAsJsonAsync(Constants.UNDELETE_FILES, requestContent, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Get file metadata.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of file metadata.</returns>
    public async Task<List<HydrusFile>> GetFileMetadataAsync(HydrusFiles files,
        CancellationToken cancellationToken = default)
    {
        return await client.GetFromHydrusAsync<List<HydrusFile>>(Constants.FILE_METADATA,
            files.ToDictionary(),
            cancellationToken);
    }

    /// <summary>
    /// Search for files based on tags.
    /// </summary>
    /// <param name="tags">List of tags to search for.</param>
    /// <param name="fileDomain">File domain to search in.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of file IDs that match the search criteria.</returns>
    public async Task<List<int>> SearchFilesAsync(IEnumerable<string> tags,
        string fileDomain = "all my files",
        CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, object>
        {
            ["tags"] = tags,
            ["file_domain"] = fileDomain
        };

        var result =
            await client.GetFromHydrusAsync<SearchFilesResult>(Constants.FILE_METADATA, request, cancellationToken);

        return result.FileIds;
    }

    private class SearchFilesResult
    {
        [JsonPropertyName("file_ids")] public required List<int> FileIds { get; set; }
    }

    /// <summary>
    /// Render a file to a specific format.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file.</param>
    /// <param name="renderFormat">The format to render the file to.</param>
    /// <param name="renderQuality">The quality/compression level of the rendered file.</param>
    /// <param name="width">The width to scale the image to.</param>
    /// <param name="height">The height to scale the image to.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The rendered file as a byte array.</returns>
    public async Task<byte[]> RenderFileAsync(string hash,
        int? renderFormat = null,
        int? renderQuality = null,
        int? width = null,
        int? height = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, object>
        {
            ["hash"] = hash
        };

        if (renderFormat.HasValue)
        {
            query["render_format"] = renderFormat.Value;
        }

        if (renderQuality.HasValue)
        {
            query["render_quality"] = renderQuality.Value;
        }

        if (width.HasValue && height.HasValue)
        {
            query["width"] = width.Value;
            query["height"] = height.Value;
        }

        var queryString = string.Join("&",
            query.Select(kvp =>
            {
                var key = Uri.EscapeDataString(kvp.Key);
                var value = kvp.Value.ToString();

                if (value is null)
                {
                    throw new NotImplementedException("write an error here");
                }

                return $"{key}={Uri.EscapeDataString(value)}";
            }));

        var response = await client.GetAsync($"get_files/render?{queryString}", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Generate hashes for a file.
    /// </summary>
    /// <param name="path">The file path to generate hashes for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A dictionary containing different types of hashes.</returns>
    public async Task<Dictionary<string, object>> GenerateHashesAsync(string path,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["path"] = path
        };

        return await client.GetFromHydrusAsync<Dictionary<string, object>>(Constants.GENERATE_HASHES,
            requestContent,
            cancellationToken);
    }

    /// <summary>
    /// Get a file's path.
    /// </summary>
    /// <param name="fileId">The numerical file ID.</param>
    /// <param name="hash">The SHA256 hash of the file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The file path.</returns>
    public async Task<string> GetFilePathAsync(int? fileId = null,
        string? hash = null,
        CancellationToken cancellationToken = default)
    {
        if ((fileId == null && hash == null) || (fileId != null && hash != null))
        {
            throw new ArgumentException("Provide either fileId or hash, but not both.");
        }

        if (hash is null)
        {
            throw new NotImplementedException("write an error here");
        }

        var escaped = Uri.EscapeDataString(hash);

        string query = fileId != null ? $"file_id={fileId.Value}" : $"hash={escaped}";

        var response = await client.GetAsync($"get_files/file_path?{query}", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.ReadFromHydrusJsonAsync<FilePathResult>(token: cancellationToken);

        return result.Path;
    }

    private class FilePathResult
    {
        [JsonPropertyName("path")] public required string Path { get; set; }

        [JsonPropertyName("filetype")] public required string Filetype { get; set; }

        [JsonPropertyName("size")] public long Size { get; set; }
    }

    /// <summary>
    /// Get file relationships for specified files.
    /// </summary>
    /// <param name="fileIds">List of file IDs.</param>
    /// <param name="hashes">List of file hashes.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A dictionary mapping file hashes to their relationships.</returns>
    public async Task<Dictionary<string, HydrusFileRelationship>> GetFileRelationshipsAsync(HydrusFiles files,
        CancellationToken cancellationToken = default)
    {
        return await client.GetFromHydrusAsync<Dictionary<string, HydrusFileRelationship>>(
            Constants.GET_FILE_RELATIONSHIPS,
            files.ToDictionary(),
            cancellationToken);
    }

    /// <summary>
    /// Set file relationships between two files.
    /// </summary>
    /// <param name="hashA">The SHA256 hash of the first file.</param>
    /// <param name="hashB">The SHA256 hash of the second file.</param>
    /// <param name="relationship">The relationship type.</param>
    /// <param name="doDefaultContentMerge">Whether to perform default content merge.</param>
    /// <param name="deleteA">Whether to delete the first file.</param>
    /// <param name="deleteB">Whether to delete the second file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the set operation.</returns>
    public async Task SetFileRelationshipsAsync(string hashA,
        string hashB,
        int relationship,
        bool doDefaultContentMerge,
        bool deleteA = false,
        bool deleteB = false,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                ["hash_a"] = hashA,
                ["hash_b"] = hashB,
                ["relationship"] = relationship,
                ["do_default_content_merge"] = doDefaultContentMerge,
                ["delete_a"] = deleteA,
                ["delete_b"] = deleteB
            }
        };

        var content = new Dictionary<string, object>
        {
            ["relationships"] = requestContent
        };

        await client.PostToHydrusAsync("manage_file_relationships/set_file_relationships", content, cancellationToken);
    }
}