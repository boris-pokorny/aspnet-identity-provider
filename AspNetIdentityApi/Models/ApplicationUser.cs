using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AspNetIdentityApi.Models {
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser {
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}