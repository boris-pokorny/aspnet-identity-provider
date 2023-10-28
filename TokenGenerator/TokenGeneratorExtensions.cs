using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace TokenGenerator;

public static class TokenGeneratorExtensions
{
    public static void AddTokenGenerator(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITokenGenerator, TokenGenerator>();
    }
}