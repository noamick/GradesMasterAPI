using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Course : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<TaskDetail> Tasks { get; set; }
        [JsonIgnore]
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<GradeWeight> GradeWeights { get; set; }
    }
}
