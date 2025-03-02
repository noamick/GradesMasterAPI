namespace backend.Dtos
{
    public class TaskCreateDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public int CourseId { get; set; }
        public string Description { get; set; }
    }
}
