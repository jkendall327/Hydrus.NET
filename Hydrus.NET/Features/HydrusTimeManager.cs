namespace Hydrus.NET;

public record HydrusViewStatistics(
    [property: JsonPropertyName("canvas_type")] int CanvasType,
    [property: JsonPropertyName("canvas_type_pretty")] string CanvasTypePretty,
    [property: JsonPropertyName("views")] int Views,
    [property: JsonPropertyName("viewtime")] float ViewTime,
    [property: JsonPropertyName("last_viewed_timestamp")] float? LastViewedTimestamp);

public sealed class HydrusTimeManager(HttpClient httpClient)
{
    /// <summary>
    /// Add a file view to the file viewing statistics system.
    /// </summary>
    /// <param name="files">File identifiers to increment views for</param>
    /// <param name="canvasType">The canvas type being edited</param>
    /// <param name="timestamp">Optional timestamp in seconds for when the viewing started</param>
    /// <param name="timestampMs">Optional timestamp in milliseconds for when the viewing started</param>
    /// <param name="views">Optional number of views to add, defaults to 1</param>
    /// <param name="viewtime">How long the user viewed the file for in seconds</param>
    public async Task IncrementFileViewtimeAsync(
        HydrusFiles files,
        int canvasType,
        double? timestamp = null,
        long? timestampMs = null,
        int? views = null,
        float viewtime = 0)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "canvas_type", canvasType },
            { "viewtime", viewtime }
        };

        if (files.FileId.HasValue)
            requestBody["file_id"] = files.FileId.Value;
        if (files.FileIds != null)
            requestBody["file_ids"] = files.FileIds;
        if (files.Hash != null)
            requestBody["hash"] = files.Hash;
        if (files.Hashes != null)
            requestBody["hashes"] = files.Hashes;
        if (timestamp.HasValue)
            requestBody["timestamp"] = timestamp.Value;
        if (timestampMs.HasValue)
            requestBody["timestamp_ms"] = timestampMs.Value;
        if (views.HasValue)
            requestBody["views"] = views.Value;

        var response = await httpClient.PostAsJsonAsync("edit_times/increment_file_viewtime", requestBody);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Set fixed values in the file viewing statistics system.
    /// </summary>
    /// <param name="files">File identifiers to set views for</param>
    /// <param name="canvasType">The canvas type being edited</param>
    /// <param name="timestamp">Optional timestamp in seconds for when the viewing occurred</param>
    /// <param name="timestampMs">Optional timestamp in milliseconds for when the viewing occurred</param>
    /// <param name="views">Number of views to set</param>
    /// <param name="viewtime">Total view time in seconds to set</param>
    public async Task SetFileViewtimeAsync(
        HydrusFiles files,
        int canvasType,
        int views,
        float viewtime,
        double? timestamp = null,
        long? timestampMs = null)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "canvas_type", canvasType },
            { "views", views },
            { "viewtime", viewtime }
        };

        if (files.FileId.HasValue)
            requestBody["file_id"] = files.FileId.Value;
        if (files.FileIds != null)
            requestBody["file_ids"] = files.FileIds;
        if (files.Hash != null)
            requestBody["hash"] = files.Hash;
        if (files.Hashes != null)
            requestBody["hashes"] = files.Hashes;
        if (timestamp.HasValue)
            requestBody["timestamp"] = timestamp.Value;
        if (timestampMs.HasValue)
            requestBody["timestamp_ms"] = timestampMs.Value;

        var response = await httpClient.PostAsJsonAsync("edit_times/set_file_viewtime", requestBody);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Add or remove timestamps associated with a file.
    /// </summary>
    /// <param name="files">File identifiers to set time for</param>
    /// <param name="timestampType">The type of timestamp being edited</param>
    /// <param name="timestamp">Optional timestamp in seconds to set</param>
    /// <param name="timestampMs">Optional timestamp in milliseconds to set</param>
    /// <param name="fileServiceKey">Required for import/delete timestamp types, the file service being edited</param>
    /// <param name="canvasType">Required for last viewed timestamps, the canvas type being edited</param>
    /// <param name="domain">Required for web domain modified times, the domain being edited</param>
    public async Task SetTimeAsync(
        HydrusFiles files,
        int timestampType,
        double? timestamp = null,
        long? timestampMs = null,
        string? fileServiceKey = null,
        int? canvasType = null,
        string? domain = null)
    {
        var requestBody = new Dictionary<string, object>
        {
            { "timestamp_type", timestampType }
        };

        if (files.FileId.HasValue)
            requestBody["file_id"] = files.FileId.Value;
        if (files.FileIds != null)
            requestBody["file_ids"] = files.FileIds;
        if (files.Hash != null)
            requestBody["hash"] = files.Hash;
        if (files.Hashes != null)
            requestBody["hashes"] = files.Hashes;
        if (timestamp.HasValue)
            requestBody["timestamp"] = timestamp.Value;
        if (timestampMs.HasValue)
            requestBody["timestamp_ms"] = timestampMs.Value;
        if (fileServiceKey != null)
            requestBody["file_service_key"] = fileServiceKey;
        if (canvasType.HasValue)
            requestBody["canvas_type"] = canvasType.Value;
        if (domain != null)
            requestBody["domain"] = domain;

        var response = await httpClient.PostAsJsonAsync("edit_times/set_time", requestBody);
        response.EnsureSuccessStatusCode();
    }

    public static class TimestampTypes
    {
        public const int FileModifiedTimeWebDomain = 0;
        public const int FileModifiedTimeHardDrive = 1;
        public const int FileImportTime = 3;
        public const int FileDeleteTime = 4;
        public const int ArchivedTime = 5;
        public const int LastViewed = 6;
        public const int FileOriginallyImportedTime = 7;
    }

    public static class CanvasTypes 
    {
        public const int MediaViewer = 0;
        public const int PreviewViewer = 1;
        public const int ClientApiViewer = 4;
    }
}