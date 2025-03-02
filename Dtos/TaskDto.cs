namespace backend.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }  // Enum as string (Assignment, Exam, etc.)
        public DateTime Date { get; set; }
        public CourseDto Course { get; set; }  // Related course details
        public string? Description { get; set; }

        // Submissions associated with the task (optional, depending on the use case)
        public List<TaskSubmissionDto> Submissions { get; set; } = new List<TaskSubmissionDto>();
    }
}
