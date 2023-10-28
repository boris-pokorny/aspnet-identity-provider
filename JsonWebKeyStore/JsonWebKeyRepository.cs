using System.Text.Json;
using Domain.Model;
using Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JsonWebKeyStore;

public class JsonWebKeyRepository : IJsonWebKeyRepository
{
    private readonly IConfiguration _configuration;

    private readonly ILogger<JsonWebKeyRepository> _logger;

    private readonly string _folderPath;
    
    public JsonWebKeyRepository(
        IConfiguration configuration,
        ILogger<JsonWebKeyRepository> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
        _folderPath = _configuration.GetValue<string>("JsonWebKeyStore:Folder") ?? "";
    }

    public async Task<string> GetJsonWebKeyAsync()
    {
        _logger.LogDebug(nameof(GetJsonWebKeyAsync));
        
        var directoryInfo = new DirectoryInfo(_folderPath);
        
        var files = directoryInfo.GetFiles().OrderBy(p => p.CreationTime).ToArray();

        var latestFile = files.FirstOrDefault();
        
        var result = await latestFile.OpenText().ReadToEndAsync();
        
        _logger.LogDebug(result);

        return result;
    }

    public async Task<IEnumerable<PublicKey>> GetPublicKeysAsync()
    {
        _logger.LogDebug(nameof(GetPublicKeysAsync));
        
        var directoryInfo = new DirectoryInfo(_folderPath);
        
        var files = directoryInfo.GetFiles().OrderBy(p => p.CreationTime).ToList();

        var result = new List<PublicKey>();

        foreach (var fileInfo in files)
        {
            var text = await fileInfo.OpenText().ReadToEndAsync();
            var key = JsonSerializer.Deserialize<PublicKey>(text, JwkSourceGenerationContext.Default.PublicKey);
            
            result.Add(key);
        }
        
        _logger.LogDebug("{}", result);

        return result;
    }
}