using Core.Entities;
using Core.Interfaces;
using Infrastructure.Helper;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppSettings _appSettings;
        public AuthenticationService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<string> Decrypt(string data)
        {
            return AESHelper.Decrypt(data,_appSettings.AESKey);
        }

        public async Task<string> Encrypt(string data)
        {
            return AESHelper.Encrypt(data, _appSettings.AESKey);
        }
    }
}
