using Hydrus.NET;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddHydrus(c =>
{
    c.BaseUrl = builder.Configuration["Hydrus:BaseUrl"];
    c.AccessKey = builder.Configuration["Hydrus:AccessKey"];
});

var app = builder.Build();

var client = app.Services.GetRequiredService<HydrusClient>();

var version = await client.Client.GetVersionAsync();

await client.Cookies.SetCookiesAsync([new("PHPSESSID", "07669eb2a1a6e840e498bb6e0799f3fb", "somesite.com", "/", 1627327719)]);
var u = await client.Cookies.GetCookiesAsync("somesite.com");

Console.WriteLine(version);