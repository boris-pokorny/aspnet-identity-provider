using Domain.Commands;
using Domain.MessageBus;
using Domain.Model;
using Domain.Ports;
using GuardNet;

namespace Domain.Handlers;

public class AuthenticateCommandHandler : IMessageHandler<AuthenticateCommand, AuthenticateResponse>
{
    private readonly IUserRepository _userRepository;

    private readonly IJsonWebKeyRepository _jsonWebKeyRepository;

    private readonly ITokenGenerator _tokenGenerator;

    public AuthenticateCommandHandler(
        IUserRepository userRepository,
        IJsonWebKeyRepository jsonWebKeyRepository,
        ITokenGenerator tokenGenerator
    )
    {
        _userRepository = userRepository;
        _jsonWebKeyRepository = jsonWebKeyRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthenticateResponse> Handle(AuthenticateCommand command, CancellationToken cancellationToken)
    {
        var handlerDictionary = new Dictionary<string, Func<AuthenticateCommand, Task<AuthenticateResponse>>>
        {
            [GrantType.Password] = HandlePasswordGrant,
            [GrantType.RefreshToken] = HandleRefreshTokenGrant,
        };

        var result = await handlerDictionary[command.GrantType](command); 

        return result;
    }

    private async Task<AuthenticateResponse> HandlePasswordGrant(AuthenticateCommand command)
    {
        var applicationUser = await _userRepository.GetUserByNameAsync(command.UserName);

        Guard.NotNull(applicationUser, "applicationUser",$"User with name {command.UserName} not found.");

        var hasPassword = await _userRepository.VerifyPasswordAsync(applicationUser, command.Password);
        
        Guard.NotEqualTo(hasPassword, true, new Exception("Wrong password."));
        
        var result = await CreateResponse(applicationUser);
        
        return result;
    }
    
    private async Task<AuthenticateResponse> HandleRefreshTokenGrant(AuthenticateCommand command)
    {
        var user = await _userRepository.GetUserByTokenAsync(command.RefreshToken);
        
        Guard.NotNull(user, "user", $"User with refresh token \"{command.RefreshToken}\" not found");

        var accessToken = await _userRepository.GetAccessTokenAsync(user);

        Guard.NotNullOrEmpty(accessToken, "accessToken", $"Access token not found.");
        
        var isExpired = _tokenGenerator.IsJwtExpired(accessToken);
        
        Guard.NotEqualTo(isExpired, false, new Exception("Access token expired."));
        
        var result = await CreateResponse(user);

        return result;
    }

    private async Task<AuthenticateResponse> CreateResponse(ApplicationUser user)
    {
        var jwk = await _jsonWebKeyRepository.GetJsonWebKeyAsync();
        var accessToken = _tokenGenerator.GenerateJwt(user, jwk);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();
        
        await _userRepository.SetRefreshTokenAsync(user, refreshToken);

        await _userRepository.SetAccessTokenAsync(user, accessToken);

        return new AuthenticateResponse
        {
            Status = AuthenticateStatus.Success,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}