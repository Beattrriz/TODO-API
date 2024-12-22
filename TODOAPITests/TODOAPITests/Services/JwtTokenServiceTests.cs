using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TODOAPI.Models;
using TODOAPI.Services;
using TODOAPI.Tests.Helper;

namespace TODOAPI.Tests.Services
{
    public class JwtTokenServiceTests
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly Mock<IConfiguration> _configurationMock;

        public JwtTokenServiceTests()
        {           
            _configurationMock = JwtTokenServiceTestHelper.GetMockedConfiguration();
            _jwtTokenService = new JwtTokenService(_configurationMock.Object);
        }

        [Fact]
        public void GenerateJwtToken_DeveRetornarTokenJwtValido()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com"
            };

            // Act
            var token = _jwtTokenService.GenerateJwtToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal("testIssuer", jwtToken.Issuer);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == user.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == "id" && c.Value == user.Id.ToString());
            Assert.Equal("testAudience", jwtToken.Audiences.FirstOrDefault());
        }
    }
}