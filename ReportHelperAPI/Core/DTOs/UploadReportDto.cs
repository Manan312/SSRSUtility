using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class UploadReportDto
    {
        public string? JobId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string TargetFolder { get; set; } = string.Empty;
        public IFormFile ZipFile { get; set; }=null!;
        public string DataSourceName { get; set; }=string.Empty;
        public string? DataSourcePath { get; set; }
    }
}
