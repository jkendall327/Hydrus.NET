using Shouldly;

namespace Hydrus.NET.Tests;

public class CookieManagementTests(HydrusContainerFixture fixture) : IClassFixture<HydrusContainerFixture>
{
    
    [Fact]
    public async Task CanRoundtripCookie()
    {
        var cookie = new HydrusCookie(
            "PHPSESSID", 
            "07669eb2a1a6e840e498bb6e0799f3fb", 
            "foobar.com", 
            "/", 
            1627327719);
        
        var client = fixture.CreateClient();
        
        await client.Cookies.SetCookiesAsync([cookie]);
        
        var actual = await client.Cookies.GetCookiesAsync("foobar.com");
        
        actual.Cookies.Single().ShouldBe(cookie);
    }
}