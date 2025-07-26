using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AuthService _authService;

        public UsersController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromQuery] string username,
            [FromQuery] string password,
            [FromQuery, SwaggerParameter("Role must be either 'Admin' or 'Customer")] string role,
            [FromQuery, SwaggerParameter("Required only if role = Customer")] string? email = null)
        {
            try
            {
                var token = await _authService.RegisterAsync(username, password, role, email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var token = await _authService.LoginAsync(username, password);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
