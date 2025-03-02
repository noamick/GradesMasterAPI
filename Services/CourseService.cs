using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all courses
        public async Task<IEnumerable<CourseDto>> GetAllCourses(string? searchQuery)
        {
            // Fetch all courses or filter by the search query
            var courses = await _context.Courses
                .Include(c => c.User)  // Include the related User entity
                .Where(c => string.IsNullOrEmpty(searchQuery) || c.Name.Contains(searchQuery))  // Allow null or empty searchQuery and return all courses
                .ToListAsync();

            // Map the courses to CourseDto
            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                User = new UserDto
                {
                    Id = c.User.Id,
                    UserName = c.User.UserName,
                    Email = c.User.Email
                }
            }).ToList();
        }


        // Get a course by its ID
        public async Task<CourseDto> GetCourseById(int id)
        {
            var course = await _context.Courses.Include(c => c.User).SingleOrDefaultAsync(c => c.Id == id);
            if (course == null) return null;

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                User = new UserDto
                {
                    Id = course.User.Id,
                    UserName = course.User.UserName,
                    Email = course.User.Email
                }
            };
        }

        // Create a new course
        public async Task<CourseDto> CreateCourse(CourseCreateDto courseDto, int userID)
        {
            var course = new Course
            {
                Name = courseDto.Name,
                Description = courseDto.Description,
                UserId = userID
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Reload the course with the User information included
            var createdCourse = await _context.Courses
                .Include(c => c.User)  // Explicitly load the User
                .SingleOrDefaultAsync(c => c.Id == course.Id);

            return new CourseDto
            {
                Id = createdCourse.Id,
                Name = createdCourse.Name,
                Description = createdCourse.Description,
                User = new UserDto
                {
                    Id = createdCourse.User.Id,
                    UserName = createdCourse.User.UserName,
                    Email = createdCourse.User.Email
                }
            };
        }

        // Update an existing course
        public async Task<CourseDto> UpdateCourse(int id, CourseUpdateDto courseDto)
        {
            var course = await _context.Courses
                .Include(c => c.User)  // Ensure the User is loaded
                .SingleOrDefaultAsync(c => c.Id == id);

            if (course == null) return null;

            // Update the course properties
            course.Name = courseDto.Name;
            course.Description = courseDto.Description;

            await _context.SaveChangesAsync();

            // Return the updated course with user information
            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                User = new UserDto
                {
                    Id = course.User.Id,
                    UserName = course.User.UserName,
                    Email = course.User.Email
                }
            };
        }


        // Delete a course by its ID
        public async Task<ServiceResult> DeleteCourse(int id)
        {
            var result = new ServiceResult();
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    result.Message = "Course not found";
                    return result;
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Course deleted successfully";
            }
            catch (Exception ex) {
                result.Message = $"Error deleting course: {ex.Message}";
            }
            return result;

        }
    }
}
