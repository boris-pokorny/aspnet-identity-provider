using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace JsonWebKeyStore;

public static class JsonWebKeyStoreExtensions
{
    public static void AddJsonWebKeyStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IJsonWebKeyRepository, JsonWebKeyRepository>();
    }
}