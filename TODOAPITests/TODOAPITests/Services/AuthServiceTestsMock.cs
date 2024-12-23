using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODOAPI.Data;
using TODOAPI.Models;
using TODOAPI.Services;
using TODOAPI.Tests.Helper;

namespace TODOAPI.Tests.Services
{
    public class AuthServiceTestsMock
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly AuthService _authService;

        public AuthServiceTestsMock(ApplicationDbContext context, Mock<IPasswordHasher<User>> passwordHasherMock, AuthService authService)
        {
            _context = context;
            _passwordHasherMock = passwordHasherMock;
            _authService = authService;
        }

        public async Task<User> CreateUserAsync(string userName, string email, string passwordHash)
        {
            var user = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public void SetupPasswordHasherToHash(string plainPassword, string hashedPassword)
        {
            _passwordHasherMock
                .Setup(ph => ph.HashPassword(null, plainPassword))
                .Returns(hashedPassword);
        }

        public void SetupPasswordHasherToVerify(User user, string plainPassword, PasswordVerificationResult result)
        {
            _passwordHasherMock
                .Setup(ph => ph.VerifyHashedPassword(user, user.PasswordHash, plainPassword))
                .Returns(result);
        }
    }
}

