using Domain.MessageBus;
using Domain.Ports;
using Domain.Queries;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers;

public class GetJsonWebKeySetQueryHandler : IMessageHandler<GetJsonWebKeySetQuery, GetJsonWebKeySetResponse>
{
    private readonly IJsonWebKeyRepository _jsonWebKeyRepository;

    private readonly ILogger<GetJsonWebKeySetQueryHandler> _logger;
    
    public GetJsonWebKeySetQueryHandler(
        IJsonWebKeyRepository jsonWebKeyRepository,
        ILogger<GetJsonWebKeySetQueryHandler> logger
    )
    {
        _jsonWebKeyRepository = jsonWebKeyRepository;
        _logger = logger;
    }

    public async Task<GetJsonWebKeySetResponse> Handle(GetJsonWebKeySetQuery query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{}()", nameof(Handle));

        var keys = await _jsonWebKeyRepository.GetPublicKeysAsync();

        var response = new GetJsonWebKeySetResponse
        {
            Keys = keys
        };
        
        _logger.LogDebug("{}() -> {}", nameof(Handle), response);
        
        return response;
    }
}