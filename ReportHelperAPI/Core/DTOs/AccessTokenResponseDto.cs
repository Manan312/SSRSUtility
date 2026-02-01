namespace Core.DTOs
{
    public class AccessTokenResponseDto
    {
        public bool? IsSuccess { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public string UserRole { get; set; } = "User";
    }
}
