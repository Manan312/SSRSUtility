using Core.DTOs;
using Core.Interfaces;
using System.Runtime.CompilerServices;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenServices _tokenServices;
        public UserService(ITokenServices tokenService)
        {
            _tokenServices = tokenService;
        }
        public async Task<AccessTokenResponseDto> GetAccessToken(LoginRequestDto loginRequest)
        {
            return await _tokenServices.GenerateAccessToken(loginRequest);
        }
    }
}
