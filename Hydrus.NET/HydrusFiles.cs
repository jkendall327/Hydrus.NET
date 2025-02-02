namespace Hydrus.NET;

/// <summary>
/// Encapsulates the common 'files' parameter to Hydrus endpoints, which can consist of one-or-more image IDs or hashes.
/// </summary>
public class HydrusFiles
{
    public int? FileId { get; set; }
    public int[]? FileIds { get; set; }
    public string? Hash { get; set; }
    public string[]? Hashes { get; set; }
}