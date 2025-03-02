namespace backend.Dtos
{
    public class StudentReportDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<CourseReportDto> Courses { get; set; }
    }
}
