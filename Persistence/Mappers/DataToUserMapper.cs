using Domain.Model;
using Persistence.Model;

namespace Persistence.Mappers;

public class DataToUserMapper : IMapper<ApplicationUserData, ApplicationUser>
{
    public void Map(ApplicationUserData source, ApplicationUser destination)
    {
        destination.Id = source.Id;
        destination.UserName = source.UserName ?? "";
    }
}