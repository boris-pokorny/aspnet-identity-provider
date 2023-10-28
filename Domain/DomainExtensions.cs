using Domain.Commands;
using Domain.Exceptions;
using Domain.Handlers;
using Domain.Queries;
using Domain.Validators;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class DomainExtensions
{
    public static void AddDomain(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRequestHandler<AuthenticateCommand, AuthenticateResponse>, AuthenticateCommandHandler>();
        
        serviceCollection.AddScoped<IRequestHandler<RegisterUserCommand, RegisterUserResponse>, RegisterUserCommandHandler>();

        serviceCollection.AddScoped<IRequestHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse>, GetJsonWebKeySetQueryHandler>();

        serviceCollection
            .AddScoped<IRequestExceptionHandler<AuthenticateCommand, AuthenticateResponse, Exception>,
                AuthenticateExceptionHandler>();

        serviceCollection
            .AddScoped<IRequestExceptionHandler<RegisterUserCommand, RegisterUserResponse, ValidationException>,
                RegisterUserValidationExceptionHandler>();

        serviceCollection
            .AddScoped<IRequestExceptionHandler<RegisterUserCommand, RegisterUserResponse, RuntimeException>,
                RegisterUserRuntimeExceptionHandler>();

        serviceCollection
            .AddScoped<IRequestExceptionHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse, Exception>,
                GetJsonWebKeySetExceptionHandler>();

        serviceCollection.AddScoped<IValidator<AuthenticateCommand>, AuthenticateCommandValidator>();

        serviceCollection.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();

        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}