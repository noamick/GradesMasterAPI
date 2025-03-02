namespace backend.Dtos
{
    public class GradeWeightDto
    {
        public int Id { get; set; }
        public CourseDto Course { get; set; }
        public string ItemType { get; set; }  // Assignment, Exam, Attendance
        public float Weight { get; set; }
    }
}
