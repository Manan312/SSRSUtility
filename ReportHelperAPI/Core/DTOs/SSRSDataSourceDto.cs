namespace Core.DTOs
{
    public class SSRSDataSourceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool IsShared { get; set; }
    }
    public class SsrsFolderDto
    {
        public string Name { get; set; }= string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
