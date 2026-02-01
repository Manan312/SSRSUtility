using Core.DTOs;

namespace Core.Interfaces
{
    public interface ITokenServices
    {
        public Task<bool> ValidateToken(string Token);
        public Task<AccessTokenResponseDto> GenerateAccessToken(LoginRequestDto loginRequest);
    }
}
