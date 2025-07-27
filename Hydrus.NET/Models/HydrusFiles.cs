namespace Hydrus.NET;

/// <summary>
/// Encapsulates the common 'files' parameter to Hydrus endpoints, which can consist of one-or-more image IDs or hashes.
/// </summary>
public class HydrusFiles
{
    public int[] FileIds { get; init; } = [];
    public string[] Hashes { get; init; } = [];

    internal Dictionary<string, object> ToDictionary()
    {
        var request = new Dictionary<string, object>();

        AddToDictionary(request);

        return request;
    }

    internal void AddToDictionary(Dictionary<string, object> request)
    {
        if (FileIds.Length is 0 && Hashes.Length is 0)
        {
            throw new InvalidOperationException("At least one file or one hash must be specified.");
        }
        
        if (FileIds.Length is 1)
        {
            request["file_id"] = FileIds[0];
        }
        else
        {
            request["file_ids"] = FileIds;
        }

        if (Hashes.Length is 1)
        {
            request["hash"] = Hashes[0];
        }
        else
        {
            request["hashes"] = Hashes;
        }
    }
}