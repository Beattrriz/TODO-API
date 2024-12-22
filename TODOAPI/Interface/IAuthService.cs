using TODOAPI.Models;

namespace TODOAPI.Interface
{
    public interface IAuthService
    {
        Task<User> RegisterUserAsync(string userName, string email, string password);
        Task<User> AuthenticateUserAsync(string email, string password);
    }
}
