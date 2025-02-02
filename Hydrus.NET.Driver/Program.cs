using Hydrus.NET;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

var app = builder.Build();

var hydrusClient = new HydrusClient(null, null);