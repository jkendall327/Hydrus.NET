namespace Hydrus.NET;

/// <summary>
/// Encapsulates the common 'files' parameter to Hydrus endpoints, which can consist of one-or-more image IDs or hashes.
/// </summary>
public class HydrusFiles
{
    public required int[] FileIds { get; init; } = [];
    public required string[] Hashes { get; init; } = [];

    private HydrusFiles()
    {
        
    }

    public static HydrusFiles Create(int[] fileIds, string[] hashes)
    {
        if (fileIds.Length is 0 && hashes.Length is 0)
        {
            throw new InvalidOperationException("At least one file or one hash must be specified.");
        }

        return new()
        {
            FileIds = fileIds,
            Hashes = hashes,
        };
    }
    
    internal Dictionary<string, object> ToDictionary()
    {
        var request = new Dictionary<string, object>();

        AddToDictionary(request);

        return request;
    }

    internal void AddToDictionary(Dictionary<string, object> request)
    {

        
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