using Shouldly;

namespace Hydrus.NET.Tests;

public class FileManagementTests(HydrusContainerFixture fixture) : IClassFixture<HydrusContainerFixture>
{
    private CancellationToken Ct => TestContext.Current.CancellationToken;
    

    
    [Fact]
    public async Task AddFileAsync_PersistsFileIntoHydrusInstance()
    {
        var client = fixture.CreateClient();
        
        var filename = "image.jpg";

        var containerFilepath = await fixture.CreateFileInContainerAsync(TestingConstants.MinimalJpeg, filename, Ct);
        
        var response = await client.Files.AddFileAsync(containerFilepath, cancellationToken: Ct);

        var filepath = await client.Files.GetFilePathAsync(fileId: response.FileId, cancellationToken: Ct);
        
        filepath.ShouldNotBeNullOrWhiteSpace();
    }
}