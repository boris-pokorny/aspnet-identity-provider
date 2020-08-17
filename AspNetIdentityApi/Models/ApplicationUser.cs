using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AspNetIdentityApi.Models {
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser {
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
    }
}