using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODOAPI.Services;

namespace TODOAPI.Tests.Helper
{
    internal class JwtTokenServiceTestHelper
    {
        public static Mock<IConfiguration> GetMockedConfiguration()
        {
            var configurationMock = new Mock<IConfiguration>();

            configurationMock.Setup(config => config["Jwt:Key"]).Returns("a-very-long-secure-key-that-is-32-chars");
            configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("testIssuer");
            configurationMock.Setup(config => config["Jwt:Audience"]).Returns("testAudience");

            return configurationMock;
        }

        public static JwtTokenService GetJwtTokenService()
        {
            var configurationMock = GetMockedConfiguration().Object;
            return new JwtTokenService(configurationMock);
        }
    }
}
