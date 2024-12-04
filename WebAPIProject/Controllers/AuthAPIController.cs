using JWTAuthLibrary.Dto;
using Microsoft.AspNetCore.Mvc;
using JWTAuthLibrary.Services;

namespace WebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized(new { message = "Invalid credentials" });
        }
    }
}
