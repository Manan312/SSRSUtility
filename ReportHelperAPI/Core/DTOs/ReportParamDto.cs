namespace Core.DTOs
{
    public class ReportParamDto
    {
        public string ReportName { get; set; }
        public List<ReportParams> ReportParams { get; set; }
    }
    public class ReportParams
    {
        public string? ParamName { get; set; }
        public string? ParamType { get; set; }
    }
}
