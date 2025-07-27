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
        CanvasTypes canvasType,
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

        files.AddToDictionary(requestBody);

        if (timestamp.HasValue)
        {
            requestBody["timestamp"] = timestamp.Value;
        }

        if (timestampMs.HasValue)
        {
            requestBody["timestamp_ms"] = timestampMs.Value;
        }

        if (views.HasValue)
        {
            requestBody["views"] = views.Value;
        }

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
        CanvasTypes canvasType,
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

        files.AddToDictionary(requestBody);

        if (timestamp.HasValue)
        {
            requestBody["timestamp"] = timestamp.Value;
        }

        if (timestampMs.HasValue)
        {
            requestBody["timestamp_ms"] = timestampMs.Value;
        }

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
        TimestampTypes timestampType,
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

        files.AddToDictionary(requestBody);

        if (timestamp.HasValue)
        {
            requestBody["timestamp"] = timestamp.Value;
        }

        if (timestampMs.HasValue)
        {
            requestBody["timestamp_ms"] = timestampMs.Value;
        }

        if (fileServiceKey != null)
        {
            requestBody["file_service_key"] = fileServiceKey;
        }

        if (canvasType.HasValue)
        {
            requestBody["canvas_type"] = canvasType.Value;
        }

        if (domain != null)
        {
            requestBody["domain"] = domain;
        }

        var response = await httpClient.PostAsJsonAsync("edit_times/set_time", requestBody);
        response.EnsureSuccessStatusCode();
    }

    public enum TimestampTypes
    {
        FileModifiedTimeWebDomain = 0,
        FileModifiedTimeHardDrive = 1,
        FileImportTime = 3,
        FileDeleteTime = 4,
        ArchivedTime = 5,
        LastViewed = 6,
        FileOriginallyImportedTime = 7
    }

    public enum CanvasTypes 
    {
        MediaViewer = 0,
        PreviewViewer = 1,
        ClientApiViewer = 4
    }
}