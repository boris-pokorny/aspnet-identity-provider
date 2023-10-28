using Domain.Commands;
using Domain.Exceptions;
using Domain.Model;
using Domain.Ports;
using GuardNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Mappers;
using Persistence.Model;
namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private const string LOGIN_PROVIDER = "Default";
    private const string ACCESS_TOKEN_NAME = "access_token";
    private readonly AspNetUserManager<ApplicationUserData> _userManager;
    private readonly IMapper<ApplicationUserData, ApplicationUser> _dataToUserMapper;
    private readonly IMapper<ApplicationUser, ApplicationUserData> _userToDataMapper;
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(
        AspNetUserManager<ApplicationUserData> userManager,
        IMapper<ApplicationUserData, ApplicationUser> dataToUserMapper,
        IMapper<ApplicationUser, ApplicationUserData> userToDataMapper,
        ILogger<UserRepository> logger
    )
    {
        _userManager = userManager;
        _dataToUserMapper = dataToUserMapper;
        _userToDataMapper = userToDataMapper;
        _logger = logger;
    }

    public async Task<ApplicationUser> AuthenticateAsync(AuthenticateCommand command)
    {
        Guard.NotNull(command, "command");
        
        var user = await _userManager.FindByNameAsync(command.UserName);

        Guard.NotNull(user, "user", $"User \"{command.UserName}\" not found.");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, command.Password);

        Guard.For(() => !isPasswordValid, new Exception("Incorrect password."));

        var result = new ApplicationUser();
        
        _dataToUserMapper.Map(user, result);
        
        return result;
    }

    public async Task<ApplicationUser> AddUserAsync(ApplicationUser user, string password)
    {
        var userData = new ApplicationUserData();
        
        _userToDataMapper.Map(user, userData);
        
        var createdResult = await _userManager.CreateAsync(userData, password);

        var errorMessage = string.Join(",", createdResult.Errors.Select(x => x.Description));
        
        Guard.For(() => !createdResult.Succeeded, new RuntimeException(errorMessage));

        return user;
    }

    public async Task<ApplicationUser?> GetUserByTokenAsync(string token)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == token);

        ApplicationUser? result = null;

        if (user == null)
        {
            _logger.LogWarning($"User with refresh token \"{token}\" not found.");
        }
        else
        {
            result = new ApplicationUser();
            _dataToUserMapper.Map(user, result);
        }

        return result;
    }

    public async Task<ApplicationUser?> GetUserByNameAsync(string userName)
    {
        var userData = await _userManager.FindByNameAsync(userName);

        ApplicationUser? result = null;

        if (userData == null)
        {
            _logger.LogWarning($"User with name \"{userName}\" not found.");
        }
        else
        {
            result = new ApplicationUser();
            _dataToUserMapper.Map(userData, result);
        }
        
        return result;
    }

    public async Task<bool> VerifyPasswordAsync(ApplicationUser user, string password)
    {
        var result = false;
        var userData = await _userManager.FindByIdAsync(user.Id);

        if (userData == null)
        {
            _logger.LogWarning($"User with Id \"{user.Id}\" not found.");
        }
        else
        {
            result = await _userManager.CheckPasswordAsync(userData, password);
        }
        
        return result;
    }

    public async Task<string> GetAccessTokenAsync(ApplicationUser user)
    {
        var userData = new ApplicationUserData();
        
        _userToDataMapper.Map(user, userData);

        var result = await _userManager.GetAuthenticationTokenAsync(userData, LOGIN_PROVIDER, ACCESS_TOKEN_NAME);
        
        return result;
        
    }

    public async Task SetRefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        var userData = await _userManager.FindByIdAsync(user.Id);
        
        if (userData == null)
        {
            _logger.LogWarning($"User with Id \"{user.Id}\" not found.");
        }
        else
        {
            userData.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(userData);
        }
    }
    
    public async Task SetAccessTokenAsync(ApplicationUser user, string accessToken)
    {
        var userData = await _userManager.FindByIdAsync(user.Id);
        
        if (userData == null)
        {
            _logger.LogWarning($"User with Id \"{user.Id}\" not found.");
        }
        else
        {
            await _userManager.SetAuthenticationTokenAsync(userData, LOGIN_PROVIDER, ACCESS_TOKEN_NAME, accessToken);
        }
    }
}