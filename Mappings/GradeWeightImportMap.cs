using backend.Dtos;
using CsvHelper.Configuration;

namespace backend.Mappings
{
    public class GradeWeightImportMap : ClassMap<GradeWeightImportDto>
    {
        public GradeWeightImportMap()
        {
            Map(m => m.CourseId).Name("course_id");
            Map(m => m.ItemType).Name("item_type");
            Map(m => m.Weight).Name("weight");
        }
    }
}
