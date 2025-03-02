using backend.Dtos;
using CsvHelper.Configuration;

public class StudentImportMap : ClassMap<StudentImportDto>
{
    public StudentImportMap()
    {
        Map(m => m.Name).Name("name");
        Map(m => m.Email).Name("email");
        Map(m => m.Phone).Name("phone");
        Map(m => m.Gender).Name("gender");
        Map(m => m.DateOfBirth).Name("date_of_birth");
        Map(m => m.Address).Name("address");
    }
}
