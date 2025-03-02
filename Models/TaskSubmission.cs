namespace backend.Models
{
    public class TaskSubmission : BaseEntity
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public TaskDetail Task { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int Grade { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Feedback { get; internal set; }
    }
}
