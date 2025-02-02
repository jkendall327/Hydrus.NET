using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hydrus.NET;

/// <summary>
/// Provides extension methods for configuring <see cref="IServiceCollection"/> to work with <see cref="HydrusClient"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="HydrusClient"/> to the service collection.
    /// Requires <see cref="HydrusOptions"/> to be registered via the options pattern.
    /// </summary>
    public static IServiceCollection AddHydrus(this IServiceCollection services)
    {
        services.AddSingleton<HydrusClient>(s =>
        {
            var options = s.GetRequiredService<IOptions<HydrusOptions>>();
            
            var client = new HttpClient
            {
                BaseAddress = new(options.Value.BaseUrl)
            };
            
            client.DefaultRequestHeaders.Add("Hydrus-Client-API-Access-Key", options.Value.AccessKey);
            
            return new(client);
        });

        return services;
    }
}