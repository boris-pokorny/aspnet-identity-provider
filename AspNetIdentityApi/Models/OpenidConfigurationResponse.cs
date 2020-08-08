using System.ComponentModel.DataAnnotations;

namespace AspNetIdentityApi.Models {
    public class OpenidConfigurationResponse {
        public string authorization_endpoint { get; set; }

        public string[] scopes_supported { get; set; }

        [Required]
        public string jwks_uri { get; set; }

    }
}