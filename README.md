# Hydrus.NET

Low-level C# client library for [the Hydrus Network](https://hydrusnetwork.github.io/hydrus/index.html)'s REST API.

I tend to make a lot of little one-off apps that interface with Hydrus and got tired writing the same boilerplate code over and over.

Unfinished.

Because it's hard to write meaningful tests for this kind of library, the .Tests projects assume a real running instance of the Hydrus client.
As such, they don't run in CI at present.
In the future, I might try to implement 'real' integration tests by spinning up an instance of the client in a container.

## Building

```bash
git clone https://github.com/yourusername/Hydrus.NET.git
cd Hydrus.NET
dotnet build
```

## Usage

NOTE: the package is not yet on NuGet.

```
dotnet add package Hydrus.NET
```

```csharp
using Hydrus.NET;

// You can retrieve these details in your client by going to:
// services -> review services -> client api ->
// 'open client api base url' / 'copy api access key'
var client = new HydrusClient("http://localhost:45869", "your-api-key");

// Import an image into the client from a web URL.
var url = "https://i.imgur.com/CLu1Svx.jpeg";

var response = await _sut.Urls.AddUrlAsync(new()
{
    Url = url
});
```

## Design

This is a low-level wrapper for the API; convenience is not currently a goal.

## Contributing

PRs are welcome.

## License

MIT License.
