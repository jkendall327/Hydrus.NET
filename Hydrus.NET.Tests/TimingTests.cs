namespace Hydrus.NET.Tests;

public class TimingTests(HydrusContainerFixture fixture, ITestOutputHelper helper)
    : IClassFixture<HydrusContainerFixture>
{
    [Fact]
    public async Task TestTimings()
    {
        var client = fixture.CreateClient();

        var file = HydrusFiles.Create([12345], []);

        helper.WriteLine("Incrementing file view time...");

        await client.Times.IncrementFileViewtimeAsync(files: file,
            canvasType: HydrusTimeManager.CanvasTypes.MediaViewer,
            views: 2,
            viewtime: 15.5f);

        helper.WriteLine("Setting file view time...");

        await client.Times.SetFileViewtimeAsync(files: file,
            canvasType: HydrusTimeManager.CanvasTypes.PreviewViewer,
            views: 10,
            viewtime: 120.0f);

        helper.WriteLine("Setting a timestamp...");

        await client.Times.SetTimeAsync(files: file,
            timestampType: HydrusTimeManager.TimestampTypes.FileImportTime,
            timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }
}