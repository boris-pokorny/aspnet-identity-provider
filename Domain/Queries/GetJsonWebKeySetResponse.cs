using Domain.Model;

namespace Domain.Queries;

public class GetJsonWebKeySetResponse
{
    public GetJsonWebKeySetStatus Status { get; set; }

    public IEnumerable<PublicKey>? Keys { get; set;  }
}