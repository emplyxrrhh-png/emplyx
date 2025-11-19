using Emplyx.ApiAdapter.Options;
using Emplyx.ApiAdapter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

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
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => (int)r.StatusCode == 429)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)));

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
