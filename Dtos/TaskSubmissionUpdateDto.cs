namespace backend.Dtos
{
    public class TaskSubmissionUpdateDto
    {
        public int Grade { get; set; }
        public string SubmissionDate { get; set; }
        public int StudentId { get; set; }
        public int TaskId { get; set; }
        public string Feedback { get; set; }
    }
}
