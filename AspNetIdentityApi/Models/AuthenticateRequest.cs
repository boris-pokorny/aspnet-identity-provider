namespace AspNetIdentityApi.Models {
    public enum EGrantType {
        client_credentials,
        refresh_token,

    }
    public class AuthenticateRequest {
        public string GrantType { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string RefreshToken { get; set; }
    }
}