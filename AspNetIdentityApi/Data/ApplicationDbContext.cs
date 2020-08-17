using AspNetIdentityApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetIdentityApi.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base (options) {

        }

        protected override void OnModelCreating (ModelBuilder builder) {
            base.OnModelCreating (builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ApplicationUser> (b => {
                // Each User can have many Tokens
                b.HasMany (e => e.Tokens)
                    .WithOne ()
                    .HasForeignKey (uc => uc.UserId)
                    .IsRequired ();
            });
        }
    }
}