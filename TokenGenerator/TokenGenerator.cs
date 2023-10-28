using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Domain.Model;
using Domain.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace TokenGenerator;

public class TokenGenerator : ITokenGenerator
{
    private readonly int _expiresIn = 3600;
    
    private readonly int _refreshExpiresIn = 60;

    private readonly ILogger<TokenGenerator> _logger;

    public TokenGenerator(ILogger<TokenGenerator> logger)
    {
        _logger = logger;
    }

    public string GenerateJwt(ApplicationUser user, string jsonWebKey)
    {
        _logger.LogDebug("GenerateJwt({user}, {jsonWebKey})", user, jsonWebKey);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwk = new JsonWebKey(jsonWebKey);
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new [] { new Claim ("id", user.Id.ToString ()) }),
            Expires = DateTime.UtcNow.AddSeconds (_expiresIn),
            SigningCredentials = new SigningCredentials (jwk, SecurityAlgorithms.RsaSha256Signature),
            IssuedAt = DateTime.UtcNow,
        };
        var token = tokenHandler.CreateToken (tokenDescriptor);
        
        var result = tokenHandler.WriteToken (token);

        _logger.LogDebug("GenerateJwt({})", result);

        return result;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        
        var rng = RandomNumberGenerator.Create();
        
        rng.GetBytes (randomNumber);
        
        var result = Convert.ToBase64String (randomNumber);

        return result;
    }
    
    public bool IsJwtExpired(string jsonWebToken)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler ();

        JwtSecurityToken securityToken = tokenHandler.ReadJwtToken (jsonWebToken);

        double timeDiff = (DateTime.UtcNow - securityToken.IssuedAt).TotalSeconds;

        var result = timeDiff > _refreshExpiresIn;
        
        return result;
    }
}