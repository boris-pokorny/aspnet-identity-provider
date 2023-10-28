using System.Reflection;
using Domain.Commands;
using Domain.Handlers;
using Domain.Model;
using Domain.Ports;
using Domain.Validators;
using FakeItEasy;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Test.Handlers;

public class AuthenticateCommandHandlerTest
{
    private ServiceProvider? _serviceProvider;

    [SetUp]
    public void Setup()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddScoped<IUserRepository>(_ => A.Fake<IUserRepository>());

        serviceCollection.AddScoped<IJsonWebKeyRepository>(_ => A.Fake<IJsonWebKeyRepository>());

        serviceCollection.AddScoped<ITokenGenerator>(_ => A.Fake<ITokenGenerator>());

        serviceCollection.AddScoped<IRequestHandler<AuthenticateCommand, AuthenticateResponse>, AuthenticateCommandHandler>();
        
        serviceCollection
            .AddScoped<IRequestExceptionHandler<AuthenticateCommand, AuthenticateResponse, Exception>,
                AuthenticateExceptionHandler>();

        serviceCollection.AddScoped<IValidator<AuthenticateCommand>, AuthenticateCommandValidator>();

        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        serviceCollection.AddLogging();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Test]
    public async Task TestHandleOk()
    {
        var command = new AuthenticateCommand
        {
            GrantType = GrantType.Password,
            UserName = "u",
            Password = "1"
        };
        var applicationUser = new ApplicationUser() { };
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>(); 
        var jsonWebKeyRepository = _serviceProvider?.GetRequiredService<IJsonWebKeyRepository>(); 
        var tokenGenerator = _serviceProvider?.GetRequiredService<ITokenGenerator>(); 
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        A.CallTo(() => userRepository.GetUserByNameAsync(command.UserName)).Returns(applicationUser);
        A.CallTo(() => userRepository.VerifyPasswordAsync(applicationUser, command.Password)).Returns(true);
        A.CallTo(() => jsonWebKeyRepository.GetJsonWebKeyAsync()).Returns("jwk");
        A.CallTo(() => tokenGenerator.GenerateJwt(applicationUser, "jwk")).Returns("token");

        var result = await mediator.Send(command);

        Assert.NotNull(result);
        Assert.AreEqual( "token", result.AccessToken);
        A.CallTo(() => userRepository.VerifyPasswordAsync(applicationUser, command.Password)).MustHaveHappened();
        A.CallTo(() => tokenGenerator.GenerateJwt(applicationUser, "jwk")).MustHaveHappened();
    }
    
    [Test]
    public async Task TestHandleInvalidUserName()
    {
        var command = new AuthenticateCommand
        {
            GrantType = GrantType.Password,
            UserName = "u",
            Password = "1"
        };

        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        var result = await mediator.Send(command);
        
        Assert.NotNull(result);
        Assert.AreEqual(AuthenticateStatus.Failure, result.Status);
    }
    
    [Test]
    public async Task TestHandleInvalidPassword()
    {
        var command = new AuthenticateCommand
        {
            GrantType = GrantType.Password,
            UserName = "u",
            Password = "1"
        };
        var applicationUser = new ApplicationUser() { };
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>(); 
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        A.CallTo(() => userRepository.GetUserByNameAsync(command.UserName)).Returns(applicationUser);
        A.CallTo(() => userRepository.VerifyPasswordAsync(applicationUser, command.Password)).Returns(false);

        var result = await mediator.Send(command);

        Assert.NotNull(result);
        Assert.AreEqual(AuthenticateStatus.Failure, result.Status);
    }

    [Test]
    public async Task TestHandleRefreshTokenInvalid()
    {
        var command = new AuthenticateCommand()
        {
            GrantType = GrantType.RefreshToken,
            RefreshToken = "X"
        };

        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();

        A.CallTo(() => userRepository.GetUserByTokenAsync(command.RefreshToken)).Returns((ApplicationUser)null);

        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        var result = await mediator.Send(command);
        
        Assert.NotNull(result);
        Assert.AreEqual(AuthenticateStatus.Failure, result.Status);
    }

    [Test]
    public async Task TestHandleRefreshTokenExpired()
    {
        var command = new AuthenticateCommand()
        {
            GrantType = GrantType.RefreshToken,
            RefreshToken = "X"
        };
        var tokenGenerator = _serviceProvider?.GetRequiredService<ITokenGenerator>(); 
        
        A.CallTo(() => tokenGenerator.IsJwtExpired(A<string>._)).Returns(true);

        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        var result = await mediator.Send(command);
        
        Assert.NotNull(result);
        Assert.AreEqual(AuthenticateStatus.Failure, result.Status);
    }

    [Test]
    public async Task TestHandleRefreshTokenOk()
    {
        var command = new AuthenticateCommand()
        {
            GrantType = GrantType.RefreshToken,
            RefreshToken = "X"
        };

        var tokenGenerator = _serviceProvider?.GetRequiredService<ITokenGenerator>(); 
        
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();
        
        A.CallTo(() => userRepository.GetAccessTokenAsync(A<ApplicationUser>._)).Returns("access_token");
        
        A.CallTo(() => tokenGenerator.IsJwtExpired("access_token")).Returns(false);

        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        var result = await mediator.Send(command);
        
        Assert.NotNull(result);
        Assert.AreEqual(AuthenticateStatus.Success, result.Status);
        Assert.NotNull(result.RefreshToken);
    }
}