using System.Reflection;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Handlers;
using Domain.Model;
using Domain.Ports;
using FakeItEasy;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Test.Handlers;

public class RegisterUserCommandHandlerTest
{
    private ServiceProvider? _serviceProvider;
    
    [SetUp]
    public void Setup()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddScoped<IRequestHandler<RegisterUserCommand, RegisterUserResponse>, RegisterUserCommandHandler>();
    
        serviceCollection
            .AddScoped<IRequestExceptionHandler<RegisterUserCommand, RegisterUserResponse, ValidationException>,
                RegisterUserValidationExceptionHandler>();
        
        serviceCollection
            .AddScoped<IRequestExceptionHandler<RegisterUserCommand, RegisterUserResponse, RuntimeException>,
                RegisterUserRuntimeExceptionHandler>();

        serviceCollection.AddScoped<IUserRepository>(_ => A.Fake<IUserRepository>());

        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        serviceCollection.AddLogging();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Test]
    public async Task HandleTestValidationException()
    {
        var mediator = _serviceProvider?.GetRequiredService<IMediator>();
        
        Assert.NotNull(mediator);

        var handler = _serviceProvider?.GetRequiredService<IRequestHandler<RegisterUserCommand, RegisterUserResponse>>();
        
        Assert.NotNull(handler);

        var command = new RegisterUserCommand();
        var response = await mediator.Send(command);

        Assert.NotNull(response);
    }
    
    [Test]
    public async Task HandleTestRuntimeException()
    {
        var mediator = _serviceProvider?.GetRequiredService<IMediator>();
        
        Assert.NotNull(mediator);

        var handler = _serviceProvider?.GetRequiredService<IRequestHandler<RegisterUserCommand, RegisterUserResponse>>();
        
        Assert.NotNull(handler);

        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();

        Assert.NotNull(userRepository);
        
        var command = new RegisterUserCommand()
        {
            UserName = "user",
            Password = "1234"
        };

        A.CallTo(() => userRepository.AddUserAsync(A<ApplicationUser>._, command.Password)).ThrowsAsync(new RuntimeException(""));

        var response = await mediator.Send(command);

        Assert.NotNull(response);
        Assert.AreEqual(RegisterUserStatus.Failure, response.Status);
    }
    
    [Test]
    public async Task HandleTestOk()
    {
        var mediator = _serviceProvider?.GetRequiredService<IMediator>();
        
        Assert.NotNull(mediator);

        var handler = _serviceProvider?.GetRequiredService<IRequestHandler<RegisterUserCommand, RegisterUserResponse>>();
        
        Assert.NotNull(handler);

        var command = new RegisterUserCommand()
        {
            UserName = "user",
            Password = "1234"
        };
        var response = await mediator.Send(command);

        Assert.NotNull(response);
    }
}