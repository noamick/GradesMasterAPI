namespace backend.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? RefreshToken { get; internal set; }
        public DateTime RefreshTokenExpiryTime { get; internal set; }
        public ICollection<Course> Courses { get; set; }
    }
}
