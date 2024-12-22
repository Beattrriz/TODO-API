using Microsoft.AspNetCore.Identity;
using Moq;
using TODOAPI.Data;
using TODOAPI.Models;
using TODOAPI.Services;
using TODOAPI.Tests.Helper;

namespace TODOAPI.Tests.Services
{
    public class AuthServiceTests
    {

        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {          
            _context = InMemoryDbContextHelper.CreateDbContext();
         
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
       
            _authService = new AuthService(_context, _passwordHasherMock.Object);
        }


        [Fact]
        public async Task RegisterUserAsync_DeveRegistrarUsuarioComSucesso()
        {
            //Arrange
            var userName = "TestUser";
            var email = "test@example.com";
            var password = "password123";

            _passwordHasherMock
            .Setup(ph => ph.HashPassword(null, password))
            .Returns("hashedPassword");

            //Act
            var result = await _authService.RegisterUserAsync(userName, email, password);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(userName, result.UserName);
            Assert.Equal(email, result.Email);
            Assert.Equal("hashedPassword", result.PasswordHash);

        }

        [Fact]
        public async Task RegisterUserAsync_DeveLancarExcecao_QuandoEmailJaExistir()
        {
            //Arrange
            var existingEmail = "existing@example.com";
            var userName = "NewUser";
            var password = "password123";

            var existingUser = new User
            {
                UserName = "ExistingUser",
                Email = existingEmail,
                PasswordHash = "hashedPassword"
            };

            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            //Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _authService.RegisterUserAsync(userName, existingEmail, password)
            );
           
            Assert.Equal("Este e-mail já está registrado.", exception.Message);
        }

        [Fact]
        public async Task AuthenticateUserAsync_DeveRetornarUsuario_SeCredenciaisForemValidas()
        {
            //Arrange
            var email = "valid@example.com";
            var password = "password123";

            var user = new User
            {
                UserName = "ValidUser",
                Email = email,
                PasswordHash = "hashedPassword"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _passwordHasherMock
            .Setup(ph => ph.VerifyHashedPassword(user, user.PasswordHash, password))
            .Returns(PasswordVerificationResult.Success);

            //Act
            var result = await _authService.AuthenticateUserAsync(email, password);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(user.UserName, result.UserName);
        }

        [Fact]
        public async Task AuthenticateUserAsync_DeveRetornarNull_SeEmailNaoForEncontrado()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "password123";

            //Act
            var result = await _authService.AuthenticateUserAsync(email, password);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_DeveRetornarNull_SeSenhaEstiverIncorreta()
        {
            // Arrange
            var email = "valid@example.com";
            var password = "wrongPassword";

            var user = new User
            {
                UserName = "ValidUser",
                Email = email,
                PasswordHash = "hashedPassword"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _passwordHasherMock
            .Setup(ph => ph.VerifyHashedPassword(user, user.PasswordHash, password))
            .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = await _authService.AuthenticateUserAsync(email, password);

            // Assert
            Assert.Null(result);
        }     
    }
}