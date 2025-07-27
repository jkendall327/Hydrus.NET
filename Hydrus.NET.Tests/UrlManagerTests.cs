using Shouldly;

namespace Hydrus.NET.Tests;

public class UrlManagerTests(HydrusContainerFixture fixture) : IClassFixture<HydrusContainerFixture>
{
    [Fact]
    public async Task CanGetFilesForKnownUrl()
    {
        var url = "https://i.imgur.com/CLu1Svx.jpeg";

        var client = fixture.CreateClient();
        
        var response = await client.Urls.GetUrlFilesAsync(url);
        
        response.NormalisedUrl.ShouldBe(url);
        
        response.UrlFileStatuses.Length.ShouldBe(1);
    }
    
    [Fact]
    public async Task CanGetInfoForKnownUrl()
    {
        var url = "https://i.imgur.com/CLu1Svx.jpeg";
        
        var client = fixture.CreateClient();
        
        var response = await client.Urls.GetUrlInfoAsync(url);
        
        response.NormalisedUrl.ShouldBe(url);
        response.UrlType.ShouldBe(UrlTypes.File);
    }

    [Fact]
    public async Task CanAddSimpleUrl()
    {
        var url = "https://i.imgur.com/CLu1Svx.jpeg";
        
        var client = fixture.CreateClient();
        
        var response = await client.Urls.AddUrlAsync(new()
        {
            Url = url
        });

        response.HumanResultText.ShouldContain("success");
    }
}