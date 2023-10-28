using Domain.Model;
using Persistence.Model;

namespace Persistence.Mappers;

public class UserToDataMapper : IMapper<ApplicationUser, ApplicationUserData>
{
    public void Map(ApplicationUser source, ApplicationUserData destination)
    {
        destination.Id = source.Id;
        destination.UserName = source.UserName;
    }
}