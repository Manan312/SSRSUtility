using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class UploadReportResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } =string.Empty;
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; } = 0;
        public List<UploadDetailsReport?>? ErrorDetails { get; set; }
    }
    public class UploadDetailsReport
    {
        public string? ReportName { get; set; }
        public string? ReportMessage { get; set; }
    }
}
