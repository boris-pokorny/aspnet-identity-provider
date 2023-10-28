using Domain.Model;

namespace Domain.Ports;

public interface ITokenGenerator
{
    string GenerateJwt(ApplicationUser user, string jsonWebKey);
    
    string GenerateRefreshToken();
    
    bool IsJwtExpired(string jsonWebToken);
}