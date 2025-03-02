namespace backend.Dtos
{
    public class GradeWeightUpdateDto
    {
        public string ItemType { get; set; }
        public float Weight { get; set; }
        public int CourseId { get; set; }
    }
}
