using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TODOAPI.DTOs;
using TODOAPI.Interface;
using TODOAPI.Services;

namespace TODO_TASK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public UserController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }
    
        [HttpPost("registro")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(registerDto.UserName, registerDto.Email, registerDto.Password);
                return Ok(new { Message = "Usuário registrado com sucesso!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
     
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            var user = await _authService.AuthenticateUserAsync(loginDto.Email, loginDto.Password);

            if(user == null)
            {
                return Unauthorized(new { Message = "E-mail ou senha inválidos." });
            }

            var token = _jwtTokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

    }
}
