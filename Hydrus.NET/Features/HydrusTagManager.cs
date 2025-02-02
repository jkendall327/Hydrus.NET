namespace Hydrus.NET;

public record HydrusTagDisplayInfo(
    [property: JsonPropertyName("ideal_tag")] string IdealTag,
    [property: JsonPropertyName("siblings")] IEnumerable<string> Siblings,
    [property: JsonPropertyName("descendants")] IEnumerable<string> Descendants,
    [property: JsonPropertyName("ancestors")] IEnumerable<string> Ancestors);
    
public record HydrusTagRelationshipsResponse(
    [property: JsonPropertyName("services")] Dictionary<string, HydrusService> Services,
    [property: JsonPropertyName("tags")] Dictionary<string, Dictionary<string, HydrusTagDisplayInfo>> Tags);

public record HydrusTagsResponse(
    [property: JsonPropertyName("tags")] IEnumerable<string> Tags);

public sealed class HydrusTagManager(HttpClient httpClient)
{
    /// <summary>
    /// Ask the client about how it will see certain tags.
    /// </summary>
    /// <param name="tags">The tags to clean</param>
    /// <returns>The cleaned tags in hydrus-friendly sorting order</returns>
    public async Task<HydrusTagsResponse> CleanTagsAsync(IEnumerable<string> tags)
    {
        // Client requires any lists in URL parameters to be percent-encoded JSON
        var encodedTags = Uri.EscapeDataString(
            System.Text.Json.JsonSerializer.Serialize(tags));

        var url = $"add_tags/clean_tags?tags={encodedTags}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusTagsResponse>();
    }

    /// <summary>
    /// Clean a single tag according to hydrus rules.
    /// </summary>
    /// <param name="tag">The tag to clean</param>
    /// <returns>The cleaned tag</returns>
    public async Task<string> CleanTagAsync(string tag)
    {
        var response = await CleanTagsAsync(new[] { tag });
        return response.Tags.First();
    }

    /// <summary>
    /// Ask the client about tags' sibling and parent relationships.
    /// </summary>
    /// <param name="tags">The tags to get relationship information for</param>
    /// <returns>A dictionary mapping tags to their relationships on each service</returns>
    public async Task<HydrusTagRelationshipsResponse> GetSiblingsAndParentsAsync(IEnumerable<string> tags)
    {
        // Client requires any lists in URL parameters to be percent-encoded JSON
        var encodedTags = Uri.EscapeDataString(
            System.Text.Json.JsonSerializer.Serialize(tags));
            
        var url = $"add_tags/get_siblings_and_parents?tags={encodedTags}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusTagRelationshipsResponse>();
    }

    /// <summary>
    /// Get relationship information for a single tag.
    /// </summary>
    /// <param name="tag">The tag to look up</param>
    /// <returns>A dictionary mapping service keys to relationship information for the tag</returns>
    public async Task<Dictionary<string, HydrusTagDisplayInfo>> GetSiblingsAndParentsAsync(string tag)
    {
        var response = await GetSiblingsAndParentsAsync([tag]);
        return response.Tags[tag];
    }
}