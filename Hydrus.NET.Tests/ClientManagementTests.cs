using Shouldly;

namespace Hydrus.NET.Tests;

public class ClientManagementTests
{
    private readonly HydrusClient _sut = TestClientCreator.CreateClient();
    
    [Fact]
    public async Task CanGetClientVersion()
    {
        var version = await _sut.Client.GetVersionAsync();
        version.Version.ShouldBe(78);
    }
}