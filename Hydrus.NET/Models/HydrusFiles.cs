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

    internal Dictionary<string, object> ToDictionary()
    {
        var requestContent = new Dictionary<string, object>();
        
        if (FileId.HasValue)
        {
            requestContent["file_id"] = FileId.Value;
        }

        if (FileIds != null)
        {
            requestContent["file_ids"] = FileIds;
        }

        if (Hash != null)
        {
            requestContent["hash"] = Hash;
        }

        if (Hashes != null)
        {
            requestContent["hashes"] = Hashes;
        }

        return requestContent;
    }

    internal void AddToDictionary(Dictionary<string, object> dict)
    {
        if (FileId.HasValue)
        {
            dict["file_id"] = FileId.Value;
        }

        if (FileIds != null)
        {
            dict["file_ids"] = FileIds;
        }

        if (Hash != null)
        {
            dict["hash"] = Hash;
        }

        if (Hashes != null)
        {
            dict["hashes"] = Hashes;
        }
    }
}