using Core.DTOs;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService : ITokenServices
    {
        private readonly AppSettings _appSettings;
        private readonly List<UserDetails> _userDetails;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        public TokenService(IOptions<AppSettings> appsettings,IOptions<List<UserDetails>> userDetails)
        { 
            _appSettings = appsettings.Value;
            _userDetails = userDetails.Value;
            _tokenHandler = new JwtSecurityTokenHandler();
        }
        public async Task<AccessTokenResponseDto> GenerateAccessToken(LoginRequestDto loginRequest)
        {
            var key = this._appSettings.JwtSecret;
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Null Credentials");
            }
            var user=_userDetails.Find(x=>x.UserName==loginRequest.UserName);
            if (user == null)
            {
                throw new NotFoundException("User Not Found"); 
            }
            var AESkey = this._appSettings.AESKey;
            if (user.Password != AESHelper.Encrypt(loginRequest.Password, AESkey)) {
                throw new NotFoundException("User Not Found");
            }

            var expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_appSettings.JwtMinutesToExpiration)==0?10: Convert.ToInt32(_appSettings.JwtMinutesToExpiration));
            var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var accessToken = this._tokenHandler.CreateToken(tokenDescriptor);
            return new AccessTokenResponseDto
            {
                IsSuccess = true,
                AccessToken = _tokenHandler.WriteToken(accessToken),
                ExpiryDate = expires,
                UserRole = string.IsNullOrEmpty(user?.Role)?"User": user?.Role
            };
        }

        public async Task<bool> ValidateToken(string accessToken)
        {
            var canReadAccessToken = this._tokenHandler.CanReadToken(accessToken);

            if (!canReadAccessToken)
            {
                return false;
            }
            SecurityToken validatedToken;
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var user = this._tokenHandler.ValidateToken(accessToken, parameters, out validatedToken);

            return validatedToken != null;
        }
    }
}
