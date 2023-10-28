using Microsoft.AspNetCore.Identity;

namespace Persistence.Model;

public class ApplicationUserData : IdentityUser
{
    public string? RefreshToken { get; set; }
}