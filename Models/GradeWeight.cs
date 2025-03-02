namespace backend.Models
{
    public class GradeWeight : BaseEntity
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string ItemType { get; set; }  // Assignment, Exam, Attendance
        public float Weight { get; set; }
    }
}
