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

Console.WriteLine(version);