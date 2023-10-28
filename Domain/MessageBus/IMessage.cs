using MediatR;

namespace Domain.MessageBus;

public interface IMessage<T> : IRequest<T>
{
    
}