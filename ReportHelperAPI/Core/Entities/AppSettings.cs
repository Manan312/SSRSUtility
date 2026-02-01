namespace Core.Entities
{
    public class AppSettings
    {
        public string? AESKey { get; set; }
        public string? JwtSecret { get; set; }
        public int JwtMinutesToExpiration { get; set; }
    }
}
