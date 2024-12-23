using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODOAPI.DTOs;
using TODOAPI.Interface;
using TODOAPI.Models;

namespace TODOAPI.Tests.Controllers
{
    public class UserControllerTestsMock
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;

        public UserControllerTestsMock(Mock<IAuthService> authServiceMock, Mock<IJwtTokenService> jwtTokenServiceMock)
        {
            _authServiceMock = authServiceMock;
            _jwtTokenServiceMock = jwtTokenServiceMock;
        }

        public RegisterDto CreateRegisterDto(string userName, string email, string password) =>
            new RegisterDto
            {
                UserName = userName,
                Email = email,
                Password = password
            };

        public LoginDto CreateLoginDto(string email, string password) =>
            new LoginDto
            {
                Email = email,
                Password = password
            };

        public User CreateUser(string userName, string email) =>
            new User
            {
                UserName = userName,
                Email = email
            };

        public void SetupRegisterUser(string userName, string email, string password, User user)
        {
            _authServiceMock
                .Setup(service => service.RegisterUserAsync(userName, email, password))
                .ReturnsAsync(user);
        }

        public void SetupRegisterUserThrows(string userName, string email, string password, Exception exception)
        {
            _authServiceMock
                .Setup(service => service.RegisterUserAsync(userName, email, password))
                .ThrowsAsync(exception);
        }

        public void SetupAuthenticateUser(string email, string password, User user)
        {
            _authServiceMock
                .Setup(service => service.AuthenticateUserAsync(email, password))
                .ReturnsAsync(user);
        }

        public void SetupGenerateJwtToken(User user, string token)
        {
            _jwtTokenServiceMock
                .Setup(service => service.GenerateJwtToken(user))
                .Returns(token);
        }
    }
}
