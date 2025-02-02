namespace Hydrus.NET;

public record HydrusNoteChanges(
    [property: JsonPropertyName("notes")] Dictionary<string, string> Notes);

public sealed class HydrusNoteManager(HttpClient httpClient)
{
    /// <summary>
    /// Add or update notes associated with a file.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file in hexadecimal</param>
    /// <param name="notes">Dictionary mapping note names to note texts</param>
    /// <param name="mergeCleverly">Whether to merge notes using tag import logic</param>
    /// <param name="extendExistingNoteIfPossible">Whether to extend existing notes when cleverly merging</param>
    /// <param name="conflictResolution">How to handle conflicts when cleverly merging (0=replace, 1=ignore, 2=append, 3=rename)</param>
    /// <returns>The notes that were actually added after any merging</returns>
    public async Task<HydrusNoteChanges> SetNotesAsync(
        string hash,
        Dictionary<string, string> notes,
        bool? mergeCleverly = null,
        bool? extendExistingNoteIfPossible = null,
        int? conflictResolution = null)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["notes"] = notes
        };

        if (mergeCleverly.HasValue)
        {
            requestContent["merge_cleverly"] = mergeCleverly.Value;
        }
        
        if (extendExistingNoteIfPossible.HasValue)
        {
            requestContent["extend_existing_note_if_possible"] = extendExistingNoteIfPossible.Value;
        }
        
        if (conflictResolution.HasValue)
        {
            requestContent["conflict_resolution"] = conflictResolution.Value;
        }

        var response = await httpClient.PostAsJsonAsync("add_notes/set_notes", requestContent);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusNoteChanges>();
    }

    /// <summary>
    /// Add or update notes associated with a file using its file ID.
    /// </summary>
    /// <param name="fileId">The numerical ID of the file</param>
    /// <param name="notes">Dictionary mapping note names to note texts</param>
    /// <param name="mergeCleverly">Whether to merge notes using tag import logic</param>
    /// <param name="extendExistingNoteIfPossible">Whether to extend existing notes when cleverly merging</param>
    /// <param name="conflictResolution">How to handle conflicts when cleverly merging (0=replace, 1=ignore, 2=append, 3=rename)</param>
    /// <returns>The notes that were actually added after any merging</returns>
    public async Task<HydrusNoteChanges> SetNotesAsync(
        int fileId,
        Dictionary<string, string> notes,
        bool? mergeCleverly = null,
        bool? extendExistingNoteIfPossible = null,
        int? conflictResolution = null)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["file_id"] = fileId,
            ["notes"] = notes
        };

        if (mergeCleverly.HasValue)
        {
            requestContent["merge_cleverly"] = mergeCleverly.Value;
        }
        
        if (extendExistingNoteIfPossible.HasValue)
        {
            requestContent["extend_existing_note_if_possible"] = extendExistingNoteIfPossible.Value;
        }
        
        if (conflictResolution.HasValue)
        {
            requestContent["conflict_resolution"] = conflictResolution.Value;
        }

        var response = await httpClient.PostAsJsonAsync("add_notes/set_notes", requestContent);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusNoteChanges>();
    }

    /// <summary>
    /// Remove notes associated with a file.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file in hexadecimal</param> 
    /// <param name="noteNames">List of note names to delete</param>
    public async Task DeleteNotesAsync(string hash, IEnumerable<string> noteNames)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["note_names"] = noteNames
        };

        var response = await httpClient.PostAsJsonAsync("add_notes/delete_notes", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Remove notes associated with a file using its file ID.
    /// </summary>
    /// <param name="fileId">The numerical ID of the file</param>
    /// <param name="noteNames">List of note names to delete</param>
    public async Task DeleteNotesAsync(int fileId, IEnumerable<string> noteNames)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["file_id"] = fileId,
            ["note_names"] = noteNames
        };

        var response = await httpClient.PostAsJsonAsync("add_notes/delete_notes", requestContent);
        response.EnsureSuccessStatusCode();
    }
}