using backend.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllStudents(string? searchQuery);
        Task<StudentDto> GetStudentById(int id);
        Task<StudentDto> CreateStudent(StudentCreateDto studentDto);
        Task<StudentDto> UpdateStudent(int id, StudentUpdateDto studentDto);
        Task<bool> DeleteStudent(int id);
        Task<ImportResult> ImportStudentsAsync(IFormFile file);
    }
}
