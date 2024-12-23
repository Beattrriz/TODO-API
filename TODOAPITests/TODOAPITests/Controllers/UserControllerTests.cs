using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TODO_TASK.Controllers;
using TODOAPI.DTOs;
using TODOAPI.Interface;
using TODOAPI.Models;

namespace TODOAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly UserController _userController;
        private readonly UserControllerTestsMock _helper;

        public UserControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _userController = new UserController(_authServiceMock.Object, _jwtTokenServiceMock.Object);
            _helper = new UserControllerTestsMock(_authServiceMock, _jwtTokenServiceMock);
        }

        [Fact]
        public async Task Register_DeveRegistrarUsuarioComSucesso()
        {
            // Arrange
            var registerDto = _helper.CreateRegisterDto("TestUser", "test@example.com", "password123");
            var user = _helper.CreateUser(registerDto.UserName, registerDto.Email);

            _helper.SetupRegisterUser(registerDto.UserName, registerDto.Email, registerDto.Password, user);

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
            var registerDto = _helper.CreateRegisterDto("TestUser", "existing@example.com", "password123");
            var exception = new InvalidOperationException("Este e-mail já está registrado.");

            _helper.SetupRegisterUserThrows(registerDto.UserName, registerDto.Email, registerDto.Password, exception);

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
            var loginDto = _helper.CreateLoginDto("test@example.com", "password123");
            var user = _helper.CreateUser("TestUser", loginDto.Email);

            _helper.SetupAuthenticateUser(loginDto.Email, loginDto.Password, user);
            _helper.SetupGenerateJwtToken(user, "mocked-jwt-token");

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
            var loginDto = _helper.CreateLoginDto("test@example.com", "wrongpassword");

            _helper.SetupAuthenticateUser(loginDto.Email, loginDto.Password, null);

            // Act
            var result = await _userController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Contains("E-mail ou senha inválidos.", unauthorizedResult.Value.ToString());
        }
    }
}