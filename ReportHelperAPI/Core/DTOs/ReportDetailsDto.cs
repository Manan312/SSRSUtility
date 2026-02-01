namespace Core.DTOs
{
    public class ReportDetailsDto
    {
        public Int32 Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
