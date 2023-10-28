using System.Reflection;
using Domain.Handlers;
using Domain.Ports;
using Domain.Queries;
using FakeItEasy;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Test.Handlers;

public class GetJsonWebKeySetQueryHandlerTest
{
    private ServiceProvider? _serviceProvider;

    [SetUp]
    public void Setup()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        serviceCollection.AddLogging();
        
        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

        serviceCollection.AddScoped<IRequestHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse>, GetJsonWebKeySetQueryHandler>();

        serviceCollection
            .AddScoped<IRequestExceptionHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse, Exception>,
                GetJsonWebKeySetExceptionHandler>();

        serviceCollection.AddScoped<IJsonWebKeyRepository>(_ => A.Fake<IJsonWebKeyRepository>());

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Test]
    public async Task HandleTestNok()
    {
        var mediator = _serviceProvider?.GetRequiredService<IMediator>();
        
        Assert.NotNull(mediator);

        var handler = _serviceProvider?.GetRequiredService<IRequestHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse>>();
        
        Assert.NotNull(handler);

        var repository = _serviceProvider?.GetRequiredService<IJsonWebKeyRepository>();

        Assert.NotNull(repository);

        var query = new GetJsonWebKeySetQuery();

        A.CallTo(() => repository.GetPublicKeysAsync()).ThrowsAsync(new Exception(""));

        var response = await mediator.Send(query);
        
        Assert.NotNull(response);
        Assert.AreEqual(GetJsonWebKeySetStatus.Failure, response.Status);
    }

    [Test]
    public async Task HandleTestOk()
    {
        var mediator = _serviceProvider?.GetRequiredService<IMediator>();
        
        Assert.NotNull(mediator);

        var handler = _serviceProvider?.GetRequiredService<IRequestHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse>>();
        
        Assert.NotNull(handler);

        var query = new GetJsonWebKeySetQuery();

        var response = await mediator.Send(query);
        
        Assert.NotNull(response);
        Assert.IsEmpty(response.Keys);
        Assert.AreEqual(GetJsonWebKeySetStatus.Success, response.Status);
    }
}