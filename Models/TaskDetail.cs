namespace backend.Models
{
    public class TaskDetail : BaseEntity
    {
        public int Id { get; set; }  // Primary key by convention
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskType Type { get; set; }  // Enum for Assignment or Exam
        public DateTime Date { get; set; }  // Date-only, no time

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<TaskSubmission> Submissions { get; set; }

        // Ensure that only the Date part is saved
        public void SetDateOnly(DateTime date)
        {
            Date = date.Date;  // Strips out the time portion
        }
    }
}
