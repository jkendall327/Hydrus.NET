namespace Hydrus.NET.Tests;

public class RatingsTests(HydrusContainerFixture fixture, ITestOutputHelper helper)
    : IClassFixture<HydrusContainerFixture>
{
    [Fact]
    public async Task Foo()
    {
        var client = fixture.CreateClient();

        // Define a file hash and some ratings
        var fileHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"; // Example SHA256 hash

        var ratings = new Dictionary<string, HydrusRating>
        {
            {
                "service_key_1", new(4.5f)
            },
            {
                "service_key_2", new(3.0f, RatingCap: 5.0f)
            }
        };

        // Add or update ratings
        helper.WriteLine("Adding/updating ratings...");
        var ratingChanges = await client.Ratings.SetRatingsAsync(fileHash, ratings);
        helper.WriteLine($"Ratings added/updated: {ratingChanges.RatingsAdded.Count}");

        // Delete ratings
        var fileId = 123456; // Example file ID

        var serviceKeysToDelete = new List<string>
        {
            "service_key_1"
        };

        helper.WriteLine("Deleting ratings...");
        await client.Ratings.DeleteRatingsAsync(fileId, serviceKeysToDelete);
        helper.WriteLine("Ratings deleted.");
    }
}