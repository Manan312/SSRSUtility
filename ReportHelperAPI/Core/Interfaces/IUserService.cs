using Core.DTOs;

namespace Core.Interfaces
{
    public interface IUserService
    {
        public Task<AccessTokenResponseDto> GetAccessToken(LoginRequestDto loginRequest);
    }
}
