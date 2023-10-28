using Domain.Exceptions;
using Domain.Model;
using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence.Test.Repositories;

public class UserRepositoryTest
{
    private ServiceProvider? _serviceProvider;
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddPersistence();

        services.AddLogging();

        _serviceProvider = services.BuildServiceProvider();
    }
    
    [Test]
    public async Task AddUserAsyncTestFail()
    {
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();

        Assert.NotNull(userRepository);

        var user = new ApplicationUser();

        Assert.ThrowsAsync<RuntimeException>(() => userRepository.AddUserAsync(user, "123"));
    }
    
    [Test]
    public async Task AddUserAsyncTestOk()
    {
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();

        Assert.NotNull(userRepository);

        var user = new ApplicationUser()
        {
            UserName = "user",
            Id = Guid.NewGuid().ToString()
        };

        var result = await userRepository.AddUserAsync(user, "Password123");
        
        Assert.NotNull(result);
    }
    
    [Test]
    public async Task GetUserByTokenAsyncTest()
    {
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();

        Assert.NotNull(userRepository);

        var user = new ApplicationUser()
        {
            UserName = "user2",
            Id = Guid.NewGuid().ToString()
        };

        var addedUser = await userRepository.AddUserAsync(user, "Password123");

        await userRepository.SetRefreshTokenAsync(addedUser, "refresh_token");
        
        var result = await userRepository.GetUserByTokenAsync("refresh_token");
        
        Assert.NotNull(result);
    }
    
    [TestCase("user_not_found", null)]
    [TestCase("user_found", "user_found")]
    public async Task GetUserByNameAsyncTest(string userName, string resultUserName)
    {
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();

        Assert.NotNull(userRepository);

        var user = new ApplicationUser()
        {
            UserName = userName,
            Id = Guid.NewGuid().ToString()
        };

        _ = await userRepository.AddUserAsync(user, "Password123");

        var result = await userRepository.GetUserByNameAsync("user_found");
        
        Assert.That(result?.UserName, Is.EqualTo(resultUserName));
    }
    
    [TestCase("IncorrectPassword12", false)]
    [TestCase("CorrectPassword1234", true)]
    public async Task HasPasswordAsyncTest(string password, bool expectedResult)
    {
        var userRepository = _serviceProvider?.GetRequiredService<IUserRepository>();

        Assert.NotNull(userRepository);

        var user = new ApplicationUser()
        {
            UserName = password,
            Id = Guid.NewGuid().ToString()
        };

        _ = await userRepository.AddUserAsync(user, password);

        var result = await userRepository.VerifyPasswordAsync(user, "CorrectPassword1234");
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }
}