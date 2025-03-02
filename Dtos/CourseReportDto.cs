using backend.Models;

namespace backend.Dtos
{
    public class CourseReportDto
    {
        public string CourseName { get; set; }
        public string Description { get; set; }
        public List<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
        public List<TaskSubmission> TaskSubmissions { get; set; } = new List<TaskSubmission>();
        public List<GradeWeight> GradeWeights { get; set; } = new List<GradeWeight>();
        public float FinalGrade { get; set; } // Calculated based on task submissions and weights
    }
}
