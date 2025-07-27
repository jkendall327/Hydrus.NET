using Shouldly;

namespace Hydrus.NET.Tests;

public class ClientManagementTests(HydrusContainerFixture fixture) : IClassFixture<HydrusContainerFixture>
{
    [Fact]
    public async Task CanGetClientVersion()
    {
        var client = fixture.CreateClient();
        var version = await client.Client.GetVersionAsync();
        version.Version.ShouldBe(80);
    }
}