namespace Core.DTOs
{
    public class UploadJobStatusDto
    {
        public string JobId { get; set; } = Guid.NewGuid().ToString();
        public string Status { get; set; } = "Queued"; // Queued | Processing | Completed | Failed
        public UploadReportResponseDto Result { get; set; } = new();
    }
}
