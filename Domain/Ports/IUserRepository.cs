using Domain.Commands;
using Domain.Model;

namespace Domain.Ports;

public interface IUserRepository
{
    Task<ApplicationUser> AddUserAsync(ApplicationUser user, string password);
    
    Task<ApplicationUser?> GetUserByTokenAsync(string token);

    Task<ApplicationUser?> GetUserByNameAsync(string userName);

    Task<bool> VerifyPasswordAsync(ApplicationUser user, string password);

    Task<string> GetAccessTokenAsync(ApplicationUser user);
    
    Task SetRefreshTokenAsync(ApplicationUser user, string refreshToken);
 
    Task SetAccessTokenAsync(ApplicationUser user, string accessToken);
}
