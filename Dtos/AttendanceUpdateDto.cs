namespace backend.Dtos
{
    public class AttendanceUpdateDto
    {
        public string Date { get; set; }
        public string Status { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
}
