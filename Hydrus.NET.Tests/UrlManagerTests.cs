using Shouldly;

namespace Hydrus.NET.Tests;

public class UrlManagerTests
{
    private readonly HydrusClient _sut = TestClientCreator.CreateClient();

    [Fact]
    public async Task CanGetFilesForKnownUrl()
    {
        var url = "https://i.imgur.com/CLu1Svx.jpeg";
        
        var response = await _sut.Urls.GetUrlFilesAsync(url);
        
        response.NormalisedUrl.ShouldBe(url);
        
        response.UrlFileStatuses.Length.ShouldBe(1);
    }
}