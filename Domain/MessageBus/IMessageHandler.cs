using MediatR;

namespace Domain.MessageBus;

public interface IMessageHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IMessage<TResponse>
{
    
}