# Hydrus.NET

Low-level C# client library for [the Hydrus Network](https://hydrusnetwork.github.io/hydrus/index.html)'s REST API.

I tend to make a lot of little one-off apps that interface with Hydrus and got tired writing the same boilerplate code over and over.

Unfinished.

Because it's hard to write meaningful tests for this kind of library, the .Tests projects assume a real running instance of the Hydrus client.
As such, they don't run in CI at present.
In the future, I might try to implement 'real' integration tests by spinning up an instance of the client in a container.