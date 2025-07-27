using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrus.NET.Tests;

public sealed class HydrusContainerFixture : IAsyncLifetime
{
    public IContainer Container { get; private set; } = null!;
    public string Host => Container.Hostname;
    public int Port => Container.GetMappedPublicPort(45869);

    public async ValueTask InitializeAsync()
    {
        // Build the container
        Container = new ContainerBuilder()
            .WithImage("ghcr.io/hydrusnetwork/hydrus:latest")
            .WithName("hydrusclient") // static container name
            .WithExposedPort(45869)
            .Build();

        await Container.StartAsync();
    }
    
    public HydrusClient CreateClient(
        string? baseUrl = "http://127.0.0.1:45869/", 
        string accessKey = "0f7990f1516a53b2af4bd717df380005e9c17e880139ce0157e85d401b027846")
    {
        IServiceCollection services = new ServiceCollection();
        
        baseUrl ??= GetBaseUrl();
        
        services.AddHydrus(c =>
        {
            c.BaseUrl = baseUrl;
            c.AccessKey = accessKey;
        });

        var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredService<HydrusClient>();

        return client;
    }

    private string GetBaseUrl()
    {
        return $"http://{Host}:{Port}/";
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }
}