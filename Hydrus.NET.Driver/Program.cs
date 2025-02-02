using Hydrus.NET;

var builder = Host.CreateApplicationBuilder();

builder.Services.Configure<HydrusOptions>(builder.Configuration.GetSection("Hydrus"));
builder.Services.AddHydrus();

var app = builder.Build();

var client = app.Services.GetRequiredService<HydrusClient>();

var version = await client.Client.GetVersionAsync();

Console.WriteLine(version);