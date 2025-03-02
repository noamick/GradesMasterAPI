namespace backend.Dtos
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public StudentDto Student { get; set; }
        public CourseDto Course { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
