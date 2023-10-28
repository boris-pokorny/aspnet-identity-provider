using Domain.Model;

namespace Domain.Ports;

public interface IJsonWebKeyRepository
{
    Task<string> GetJsonWebKeyAsync();

    Task<IEnumerable<PublicKey>> GetPublicKeysAsync();
}