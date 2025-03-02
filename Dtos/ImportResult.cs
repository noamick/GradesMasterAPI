namespace backend.Dtos
{
    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<object> Errors { get; set; } = new List<object>();
    }
}
