using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TODO_TASK.Controllers;
using TODOAPI.DTOs;
using TODOAPI.Interface;
using TODOAPI.Models;
using TODOAPI.Services;
using TODOAPI.Tests.Helper;

namespace TODOAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _userController = new UserController(_authServiceMock.Object, _jwtTokenServiceMock.Object);
        }

        [Fact]
        public async Task Register_DeveRetornarOk_SeRegistroForBemSucedido()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "TestUser",
                Email = "test@example.com",
                Password = "password123"
            };

            _authServiceMock
                .Setup(service => service.RegisterUserAsync(registerDto.UserName, registerDto.Email, registerDto.Password))
                .ReturnsAsync(new User { UserName = registerDto.UserName, Email = registerDto.Email });

            // Act
            var result = await _userController.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Contains("Usuário registrado com sucesso!", okResult.Value.ToString());
        }

        [Fact]
        public async Task Register_DeveRetornarBadRequest_SeEmailJaExistir()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "TestUser",
                Email = "existing@example.com",
                Password = "password123"
            };

            _authServiceMock
                .Setup(service => service.RegisterUserAsync(registerDto.UserName, registerDto.Email, registerDto.Password))
                .ThrowsAsync(new InvalidOperationException("Este e-mail já está registrado."));

            // Act
            var result = await _userController.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Este e-mail já está registrado.", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Login_DeveRetornarOk_SeLoginForBemSucedido()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new User
            {
                UserName = "TestUser",
                Email = loginDto.Email
            };

            _authServiceMock
                .Setup(service => service.AuthenticateUserAsync(loginDto.Email, loginDto.Password))
                .ReturnsAsync(user);

            _jwtTokenServiceMock
                .Setup(service => service.GenerateJwtToken(user))
                .Returns("mocked-jwt-token");

            // Act
            var result = await _userController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Contains("mocked-jwt-token", okResult.Value.ToString());
        }

        [Fact]
        public async Task Login_DeveRetornarUnauthorized_SeCredenciaisEstiveremErradas()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _authServiceMock
                .Setup(service => service.AuthenticateUserAsync(loginDto.Email, loginDto.Password))
                .ReturnsAsync((User)null);  

            // Act
            var result = await _userController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Contains("E-mail ou senha inválidos.", unauthorizedResult.Value.ToString());
        }     
    }
}