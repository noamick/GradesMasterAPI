namespace backend.Dtos
{
    public class TaskSubmissionCreateDto
    {
        public int Grade { get; set; }
        public string SubmissionDate { get; set; }  // ISO date string
        public int StudentId { get; set; }
        public int TaskId { get; set; }
        public string Feedback { get; set; }
    }
}
