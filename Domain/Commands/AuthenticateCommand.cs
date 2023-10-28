using Domain.MessageBus;

namespace Domain.Commands;

public sealed class AuthenticateCommand : IMessage<AuthenticateResponse>
{
    public string GrantType { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? RefreshToken { get; set; }
}