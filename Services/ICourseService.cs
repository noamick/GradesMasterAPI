using backend.Dtos;
using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface ICourseService
    {
        // Get all courses
        Task<IEnumerable<CourseDto>> GetAllCourses(string searchQuery);

        // Get a course by its ID
        Task<CourseDto> GetCourseById(int id);

        // Create a new course
        Task<CourseDto> CreateCourse(CourseCreateDto courseDto, int userID);

        // Update an existing course
        Task<CourseDto> UpdateCourse(int id, CourseUpdateDto courseDto);

        // Delete a course by its ID
        Task<ServiceResult> DeleteCourse(int id);
    }
}
