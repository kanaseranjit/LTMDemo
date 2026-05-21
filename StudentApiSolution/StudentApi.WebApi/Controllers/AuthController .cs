using Microsoft.AspNetCore.Mvc;
using StudentApi.Domain.Entities;

namespace StudentApi.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly JwtTokenService _jwtService;
        public AuthController(JwtTokenService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login.Username == "admin" && login.Password == "password") // Replace with DB check
            {
                var token = _jwtService.GenerateToken(login.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }
    }
}
