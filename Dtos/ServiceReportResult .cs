namespace backend.Dtos
{
    public class ServiceReportResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
