using Hydrus.NET;

var builder = Host.CreateApplicationBuilder();

var app = builder.Build();

var baseUrl = builder.Configuration["Hydrus:BaseUrl"];
var accessKey = builder.Configuration["Hydrus:AccessKey"];

var hydrusClient = new HydrusClient(baseUrl, accessKey);

var version = await hydrusClient.Client.GetVersionAsync();

Console.WriteLine(version);