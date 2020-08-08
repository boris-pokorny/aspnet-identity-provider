using System;
using System.IO.Abstractions;
using AspNetIdentityApi.Controllers;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public class DiscoveryController_Test {
        private readonly DiscoveryController _controller;

        public DiscoveryController_Test () {
            var configuration = new Mock<IConfiguration> ();
            var configurationSection = new Mock<IConfigurationSection> ();
            var fs = new Mock<IFileSystem> ();
            var jwkHelper = new JwkHelper ();

            configurationSection.Setup (a => a.Value).Returns ("url");
            configuration.Setup (a => a.GetSection ("ASPNETCORE_URLS")).Returns (configurationSection.Object);
            configuration.Setup (a => a.GetSection ("JwkFile")).Returns (configurationSection.Object);

            fs.Setup (f => f.File.ReadAllText (It.IsAny<String> ())).Returns (jwkHelper.jsonSerializedKey);

            _controller = new DiscoveryController (configuration.Object, fs.Object);

        }

        [Fact]
        public void TestOpenidConfiguration () {
            var result = _controller.OpenidConfiguration ();
            var okResult = (OkObjectResult) result;
            var value = (OpenidConfigurationResponse) okResult.Value;

            Assert.IsType<OpenidConfigurationResponse> (okResult.Value);
            Assert.Equal ("url/.well-known/openid-configuration/jwks", value.jwks_uri);
        }

        [Fact]
        public void TestOpenidConfigurationJwks () {
            var result = _controller.OpenidConfigurationJwks ();
            var okResult = (OkObjectResult) result;
            var value = (JsonWebKeySet) okResult.Value;

            Assert.Equal (1, value.Keys.Count);
            Assert.False (value.Keys[0].HasPrivateKey);
        }
    }
}