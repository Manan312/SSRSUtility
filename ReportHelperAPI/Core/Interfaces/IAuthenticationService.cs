namespace Core.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<string> Encrypt(string data);
        public Task<string> Decrypt(string data);
    }
}
