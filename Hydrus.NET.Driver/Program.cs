using Hydrus.NET;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddHydrus(c =>
{
    c.BaseUrl = builder.Configuration["Hydrus:BaseUrl"];
    c.AccessKey = builder.Configuration["Hydrus:AccessKey"];
});

var app = builder.Build();

var client = app.Services.GetRequiredService<HydrusClient>();

await TestClientManagement(client);

await TestCookies(client);

await TestTimes(client.Times);

return;

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

async Task TestClientManagement(HydrusClient hydrusClient)
{
    var hydrusVersion = await hydrusClient.Client.GetVersionAsync();
    
    Console.WriteLine(hydrusVersion);
}

async Task TestCookies(HydrusClient hydrusClient)
{
    var cookie = new HydrusCookie("PHPSESSID", "07669eb2a1a6e840e498bb6e0799f3fb", "somesite.com", "/", 1627327719);
    await hydrusClient.Cookies.SetCookiesAsync([cookie]);
    var u = await hydrusClient.Cookies.GetCookiesAsync("somesite.com");
}