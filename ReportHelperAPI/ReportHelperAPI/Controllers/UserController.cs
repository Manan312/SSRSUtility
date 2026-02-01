using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ReportHelperAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var accessToken = await _userService.GetAccessToken(loginRequest);
            return Ok(accessToken);
        }
    }
}
