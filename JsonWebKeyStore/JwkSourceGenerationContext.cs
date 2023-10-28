using System.Text.Json.Serialization;
using Domain.Model;

namespace JsonWebKeyStore;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(PublicKey))]
internal partial class JwkSourceGenerationContext : JsonSerializerContext
{
}
