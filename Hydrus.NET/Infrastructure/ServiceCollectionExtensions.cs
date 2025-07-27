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
    public static IServiceCollection AddHydrus(
        this IServiceCollection services,
        Action<HydrusOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<HydrusClient>(s =>
        {
            var options = s.GetRequiredService<IOptions<HydrusOptions>>().Value;
            
            var client = new HttpClient();
            client.BaseAddress = new(options.BaseUrl);
            client.DefaultRequestHeaders.Add("Hydrus-Client-API-Access-Key", options.AccessKey);
            
            return new(client);
        });

        return services;
    }
}