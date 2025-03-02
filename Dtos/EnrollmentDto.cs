namespace backend.Dtos
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public CourseDto Course { get; set; }
        public StudentDto Student { get; set; }
    }
}
