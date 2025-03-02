namespace backend.Dtos
{
    public class TaskSubmissionDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int StudentId { get; set; }  // Include StudentId here
        public StudentDto Student { get; set; }  // Associated student information
        public int Grade { get; set; }  // Grade for the submission
        public DateTime SubmissionDate { get; set; }  // Date of submission
    }
}
