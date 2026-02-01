namespace Core.DTOs
{
    public class ReportDownloadResultDto
    {
        public byte[] Content { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/xml";
    }
}
