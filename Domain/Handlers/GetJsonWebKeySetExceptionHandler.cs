using Domain.Queries;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers;

public class GetJsonWebKeySetExceptionHandler : IRequestExceptionHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse, Exception>
{
    private readonly ILogger<GetJsonWebKeySetExceptionHandler> _logger;

    public GetJsonWebKeySetExceptionHandler(ILogger<GetJsonWebKeySetExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(
        GetJsonWebKeySetQuery request,
        Exception exception,
        RequestExceptionHandlerState<GetJsonWebKeySetResponse> state,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception.Message);

        state.SetHandled(new GetJsonWebKeySetResponse()
        {
            Status = GetJsonWebKeySetStatus.Failure
        });
    }
}