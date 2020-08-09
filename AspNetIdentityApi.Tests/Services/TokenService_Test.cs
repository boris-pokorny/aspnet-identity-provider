using System;
using System.IO.Abstractions;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Services;
using AspNetIdentityApi.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public class TokenService_Test {
        private readonly ITokenService _service;

        public TokenService_Test () {

            var signInManager = new FakeSignInManager ();
            var configuration = new Mock<IConfiguration> ();
            var configurationSection = new Mock<IConfigurationSection> ();
            var sectionTokens = new Mock<IConfigurationSection> ();
            var sectionJwtFile = new Mock<IConfigurationSection> ();
            var sectionExpiresIn = new Mock<IConfigurationSection> ();

            var fs = new Mock<IFileSystem> ();
            var jwkHelper = new JwkHelper ();

            sectionJwtFile.Setup (a => a.Value).Returns ("tempkey.jwk");
            sectionExpiresIn.Setup (a => a.Value).Returns ("3600");
            sectionTokens.Setup (a => a.GetSection ("JwkFile")).Returns (sectionJwtFile.Object);
            sectionTokens.Setup (a => a.GetSection ("ExpiresIn")).Returns (sectionExpiresIn.Object);
            configuration.Setup (a => a.GetSection ("Tokens")).Returns (sectionTokens.Object);

            fs.Setup (f => f.File.ReadAllText (It.IsAny<String> ())).Returns (jwkHelper.jsonSerializedKey);

            _service = new TokenService (configuration.Object, fs.Object);
        }

        [Fact]
        public void TestGenerateJwtToken () {

            var user = new ApplicationUser { };
            var token = _service.GenerateJwtToken (user);

            Assert.IsType<string> (token);
            Assert.Equal (3, token.Split (".").Length);
        }

        [Fact]
        public void TestGenerateRefreshToken () {

            var token = _service.GenerateRefreshToken ();

            Assert.IsType<string> (token);
            Assert.Equal (44, token.Length);
        }

        [Fact]
        public void TestCreateResponse () {

            var user = new ApplicationUser { };
            var response = _service.CreateResponse (user, "", "");

            Assert.IsType<TokenResponse> (response);
            Assert.Equal (3600, response.expires_in);
        }
    }
}