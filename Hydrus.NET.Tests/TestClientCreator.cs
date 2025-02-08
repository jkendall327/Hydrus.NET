using Microsoft.Extensions.DependencyInjection;

namespace Hydrus.NET.Tests;

public class TestClientCreator
{
    public static HydrusClient CreateClient(
        string baseUrl = "http://127.0.0.1:45869/", 
        string accessKey = "0f7990f1516a53b2af4bd717df380005e9c17e880139ce0157e85d401b027846")
    {
        IServiceCollection services = new ServiceCollection();
        
        services.AddHydrus(c =>
        {
            c.BaseUrl = baseUrl;
            c.AccessKey = accessKey;
        });

        var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredService<HydrusClient>();

        return client;
    }
}