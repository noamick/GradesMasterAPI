namespace backend.Dtos
{
    public class GradeWeightCreateDto
    {
        public string ItemType { get; set; }
        public float Weight { get; set; }
        public int CourseId { get; set; }
    }
}
