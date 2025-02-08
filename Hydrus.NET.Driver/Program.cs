using Hydrus.NET;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddHydrus(c =>
{
    c.BaseUrl = builder.Configuration["Hydrus:BaseUrl"];
    c.AccessKey = builder.Configuration["Hydrus:AccessKey"];
});

var app = builder.Build();

var client = app.Services.GetRequiredService<HydrusClient>();

await TestTimes(client.Times);

await TestRatings(client);

return;

async Task TestRatings(HydrusClient hydrusClient)
{
    var ratingManager = hydrusClient.Ratings;

    // Define a file hash and some ratings
    var fileHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"; // Example SHA256 hash
    var ratings = new Dictionary<string, HydrusRating>
    {
        { "service_key_1", new(4.5f) },
        { "service_key_2", new(3.0f, RatingCap: 5.0f) }
    };

    // Add or update ratings
    Console.WriteLine("Adding/updating ratings...");
    var ratingChanges = await ratingManager.SetRatingsAsync(fileHash, ratings);
    Console.WriteLine($"Ratings added/updated: {ratingChanges.RatingsAdded.Count}");

    // Delete ratings
    var fileId = 123456; // Example file ID
    var serviceKeysToDelete = new List<string> { "service_key_1" };
    Console.WriteLine("Deleting ratings...");
    await ratingManager.DeleteRatingsAsync(fileId, serviceKeysToDelete);
    Console.WriteLine("Ratings deleted.");
}


async Task TestTimes(HydrusTimeManager timeManager)
{
    var file = new HydrusFiles { FileId = 123456 };

    Console.WriteLine("Incrementing file view time...");
    await timeManager.IncrementFileViewtimeAsync(
        files: file,
        canvasType: HydrusTimeManager.CanvasTypes.MediaViewer,
        views: 2,
        viewtime: 15.5f
    );

    Console.WriteLine("Setting file view time...");
    await timeManager.SetFileViewtimeAsync(
        files: file,
        canvasType: HydrusTimeManager.CanvasTypes.PreviewViewer,
        views: 10,
        viewtime: 120.0f
    );

    Console.WriteLine("Setting a timestamp...");
    await timeManager.SetTimeAsync(
        files: file,
        timestampType: HydrusTimeManager.TimestampTypes.FileImportTime,
        timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    );
}