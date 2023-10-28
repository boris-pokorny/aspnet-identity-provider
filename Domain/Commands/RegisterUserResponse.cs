namespace Domain.Commands;

public sealed class RegisterUserResponse
{
    public RegisterUserStatus Status { get; set; }
    
    public string? Error { get; set; }
}