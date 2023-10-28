using Domain.Model;
using Domain.Ports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Mappers;
using Persistence.Model;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceExtensions
{
    public static void AddPersistence(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<DbContext, IdentityDbContext<ApplicationUserData>>();
        
        serviceCollection.AddDbContext<IdentityDbContext<ApplicationUserData>>(options => options.UseInMemoryDatabase("UserStore"));

        serviceCollection.AddIdentityCore<ApplicationUserData>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddUserStore<UserStore<ApplicationUserData>>()
        .AddUserManager<AspNetUserManager<ApplicationUserData>>();

        serviceCollection.AddScoped<IMapper<ApplicationUserData, ApplicationUser>, DataToUserMapper>();
        
        serviceCollection.AddScoped<IMapper<ApplicationUser, ApplicationUserData>, UserToDataMapper>();

        serviceCollection.AddScoped<IUserRepository, UserRepository>();
    }
}