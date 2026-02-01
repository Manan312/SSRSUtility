namespace Core.DTOs
{
    public class UploadJobResponseDto
    {
        public string JobId { get; set; }
        public string Status { get; set; }     // Queued | Processing | Completed | Failed     // 0–100
        public string? Message { get; set; }
    }
}
