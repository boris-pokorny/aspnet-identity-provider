namespace Domain.Model;

public sealed class PublicKey
{
    // Key ID
    public string Kid { get; set; }

    // Key Type
    public string Kty { get; set; }

    // Exponent
    public string E { get; set; }
    
    // Modulus
    public string N { get; set; }

}