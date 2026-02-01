using Core.DTOs;
using Core.Interfaces;
using Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReportHelperAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [SkipResponseMiddlewareAttribute]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        { 
            _authenticationService = authenticationService;
        }
        [HttpPost("Encrypt")]
        public async Task<ActionResult> EncryptData([FromBody] DataRequestDto dataRequestDto)
        {
            var encryptedValue = await _authenticationService.Encrypt(dataRequestDto.Token);
            return Ok(encryptedValue);
        }
        [HttpPost("Decrypt")]
        public async Task<ActionResult> DecryptData([FromBody] DataRequestDto dataRequestDto)
        {
            var decryptedValue = await _authenticationService.Decrypt(dataRequestDto.Token);
            return Ok(decryptedValue);
        }
    }
}
