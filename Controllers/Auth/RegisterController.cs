using Microsoft.AspNetCore.Mvc;
using Trace.DTO.Auth;
using Trace.Service.Auth.GeneralAuth;

namespace Trace.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var result = await _userService.RegisterUser(model);
            if (result.Succeeded)
                return Ok("User registered successfully!");

            return BadRequest(result.Errors);
        }
    }
}
