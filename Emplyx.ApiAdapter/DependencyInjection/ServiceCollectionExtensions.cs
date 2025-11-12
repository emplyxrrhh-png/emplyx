using Emplyx.ApiAdapter.Options;
using Emplyx.ApiAdapter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Emplyx.ApiAdapter.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmplyxApiAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiAdapterOptions>(configuration.GetSection("ApiAdapter"));

        services.AddHttpClient<IApiAdapterClient, ApiAdapterClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiAdapterOptions>>().Value;
            var baseAddress = options.ResolveBaseAddress();
            if (baseAddress is not null)
            {
                client.BaseAddress = baseAddress;
            }

            client.Timeout = options.ResolveTimeout();
        });

        return services;
    }
}
