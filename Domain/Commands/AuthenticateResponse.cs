namespace Domain.Commands;

public sealed class AuthenticateResponse
{
    public AuthenticateStatus Status { get; set; }
    
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}