using TODOAPI.Models;

namespace TODOAPI.Interface
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user);
    }
}
