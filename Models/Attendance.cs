namespace backend.Models
{
    public class Attendance : BaseEntity
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime Date { get; set; }

        public AttendanceStatus Status { get; set; }
    }

    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late
    }
}
