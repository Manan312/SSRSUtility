namespace Core.DTOs
{
    public class DownloadReportRequestDto
    {
        public string? token { get; set; }
        public string? folderPath { get; set; }
        public List<string> reportNames { get; set; }
    }
}
