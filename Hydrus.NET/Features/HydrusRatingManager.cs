namespace Hydrus.NET;

public record HydrusRating(
    [property: JsonPropertyName("rating")] float Rating,
    [property: JsonPropertyName("rating_cap")] float? RatingCap = null,
    [property: JsonPropertyName("rating_count_cap")] int? RatingCountCap = null,
    [property: JsonPropertyName("shape")] float? Shape = null);

public record HydrusRatingChanges(
    [property: JsonPropertyName("ratings_added")] Dictionary<string, HydrusRating> RatingsAdded);

public sealed class HydrusRatingManager(HttpClient httpClient)
{
    /// <summary>
    /// Add or update ratings associated with a file using its hash.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file in hexadecimal</param>
    /// <param name="ratings">Dictionary mapping service keys to rating objects</param>
    /// <param name="preventSpaceRatings">Whether to block any rating intended for namespaces</param>
    /// <returns>The ratings that were actually added</returns>
    public async Task<HydrusRatingChanges> SetRatingsAsync(
        string hash,
        Dictionary<string, HydrusRating> ratings,
        bool? preventSpaceRatings = null)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["ratings"] = ratings
        };

        if (preventSpaceRatings.HasValue)
        {
            requestContent["prevent_space_ratings"] = preventSpaceRatings.Value;
        }

        var response = await httpClient.PostAsJsonAsync("add_ratings/set_rating", requestContent);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusRatingChanges>();
    }

    /// <summary>
    /// Add or update ratings associated with a file using its file ID.
    /// </summary>
    /// <param name="fileId">The numerical ID of the file</param>
    /// <param name="ratings">Dictionary mapping service keys to rating objects</param>
    /// <param name="preventSpaceRatings">Whether to block any rating intended for namespaces</param>
    /// <returns>The ratings that were actually added</returns>
    public async Task<HydrusRatingChanges> SetRatingsAsync(
        int fileId,
        Dictionary<string, HydrusRating> ratings,
        bool? preventSpaceRatings = null)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["file_id"] = fileId,
            ["ratings"] = ratings
        };

        if (preventSpaceRatings.HasValue)
        {
            requestContent["prevent_space_ratings"] = preventSpaceRatings.Value;
        }

        var response = await httpClient.PostAsJsonAsync("add_ratings/set_rating", requestContent);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusRatingChanges>();
    }

    /// <summary>
    /// Remove ratings associated with a file using its hash.
    /// </summary>
    /// <param name="hash">The SHA256 hash of the file in hexadecimal</param>
    /// <param name="serviceKeys">List of service keys for ratings to delete</param>
    public async Task DeleteRatingsAsync(string hash, IEnumerable<string> serviceKeys)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["hash"] = hash,
            ["service_keys"] = serviceKeys
        };

        var response = await httpClient.PostAsJsonAsync("add_ratings/delete_ratings", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Remove ratings associated with a file using its file ID.
    /// </summary>
    /// <param name="fileId">The numerical ID of the file</param>
    /// <param name="serviceKeys">List of service keys for ratings to delete</param>
    public async Task DeleteRatingsAsync(int fileId, IEnumerable<string> serviceKeys)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["file_id"] = fileId,
            ["service_keys"] = serviceKeys
        };

        var response = await httpClient.PostAsJsonAsync("add_ratings/delete_ratings", requestContent);
        response.EnsureSuccessStatusCode();
    }
}