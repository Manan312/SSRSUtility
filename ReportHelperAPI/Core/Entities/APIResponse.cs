namespace Core.Entities
{
    public class APIResponse<T>
    {
        public bool Success { get; set; } = true;
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public string? TraceId { get; set; }
    }
}
