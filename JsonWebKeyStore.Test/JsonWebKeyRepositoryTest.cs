using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JsonWebKeyStore.Test;

public class Tests
{
    private JsonWebKeyRepository? _repository;

    [SetUp]
    public void Setup()
    {
        var fakeConfigSection = A.Fake<IConfigurationSection>();
        
        A.CallTo(() => fakeConfigSection.Value).Returns("../../../ExampleKeySet");

        var fakeConfig = A.Fake<IConfiguration>();

        A.CallTo(() => fakeConfig.GetSection("JsonWebKeyStore:Folder"))
            .Returns(fakeConfigSection);
        
        _repository = new JsonWebKeyRepository(
            fakeConfig,
            A.Fake<ILogger<JsonWebKeyRepository>>()
        );
    }

    [Test]
    public async Task GetJsonWebKeyTest()
    {
        var jwk = await _repository.GetJsonWebKeyAsync();
        
        Assert.IsNotEmpty(jwk);;
    }
    
    [Test]
    public async Task GetPublicKeysAsyncTest()
    {
        var keys = await _repository.GetPublicKeysAsync();
        
        Assert.IsNotEmpty(keys);;
    }
}