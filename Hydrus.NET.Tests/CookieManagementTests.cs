using Shouldly;

namespace Hydrus.NET.Tests;

public class CookieManagementTests
{
    private readonly HydrusClient _sut = TestClientCreator.CreateClient();
    
    [Fact]
    public async Task CanRoundtripCookie()
    {
        var cookie = new HydrusCookie(
            "PHPSESSID", 
            "07669eb2a1a6e840e498bb6e0799f3fb", 
            "foobar.com", 
            "/", 
            1627327719);
        
        await _sut.Cookies.SetCookiesAsync([cookie]);
        
        var actual = await _sut.Cookies.GetCookiesAsync("foobar.com");
        
        actual.Cookies.Single().ShouldBe(cookie);
    }
}