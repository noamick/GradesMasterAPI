using backend.Data;
using backend.Dtos;
using backend.Models;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all students or search by name
        public async Task<IEnumerable<StudentDto>> GetAllStudents(string? searchQuery)
        {
            var students = await _context.Students
                .Include(s => s.Enrollments).ThenInclude(e => e.Course)  // Include courses through enrollments
                .Where(s => string.IsNullOrEmpty(searchQuery) || s.Name.Contains(searchQuery))
                .ToListAsync();

            return students.Select(s => new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Address = s.Address,
                Phone = s.Phone,
                Gender = s.Gender,
                DateOfBirth = s.DateOfBirth,

                // Map full course details instead of just CourseIds
                Courses = s.Enrollments.Select(e => new CourseDto
                {
                    Id = e.Course.Id,
                    Name = e.Course.Name,
                    Description = e.Course.Description
                }).ToList()
            }).ToList();
        }


        // Get a student by ID
        public async Task<StudentDto> GetStudentById(int id)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments).ThenInclude(e => e.Course)  // Include course details via enrollments
                .SingleOrDefaultAsync(s => s.Id == id);

            if (student == null) return null;

            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Address = student.Address,
                Phone = student.Phone,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,

                // Map full course details instead of just CourseIds
                Courses = student.Enrollments.Select(e => new CourseDto
                {
                    Id = e.Course.Id,
                    Name = e.Course.Name,
                    Description = e.Course.Description
                }).ToList()
            };
        }


        // Create a new student
        public async Task<StudentDto> CreateStudent(StudentCreateDto studentDto)
        {
            var student = new Student
            {
                Name = studentDto.Name,
                Email = studentDto.Email,
                Address = studentDto.Address,
                Phone = studentDto.Phone,
                Gender = studentDto.Gender,
                DateOfBirth = studentDto.DateOfBirth
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Add student enrollments
            var existingEnrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id && studentDto.CourseIds.Contains(e.CourseId))
                .Select(e => e.CourseId)
                .ToListAsync();

            var newCourseIds = studentDto.CourseIds.Except(existingEnrollments).ToList();

            foreach (var courseId in newCourseIds)
            {
                var enrollment = new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = courseId
                };
                _context.Enrollments.Add(enrollment);
            }

            await _context.SaveChangesAsync();

            // Fetch full course details for the enrolled courses
            var enrolledCourses = await _context.Courses
                .Where(c => studentDto.CourseIds.Contains(c.Id))
                .ToListAsync();

            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Address = student.Address,
                Phone = student.Phone,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                Courses = enrolledCourses.Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };
        }

        // Update an existing student
        public async Task<StudentDto> UpdateStudent(int id, StudentUpdateDto studentDto)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .SingleOrDefaultAsync(s => s.Id == id);

            if (student == null) return null;

            student.Name = studentDto.Name;
            student.Email = studentDto.Email;
            student.Address = studentDto.Address;
            student.Phone = studentDto.Phone;
            student.Gender = studentDto.Gender;
            student.DateOfBirth = studentDto.DateOfBirth;

            // Remove existing enrollments
            var existingEnrollments = student.Enrollments.ToList();
            _context.Enrollments.RemoveRange(existingEnrollments);

            // Add updated enrollments
            foreach (var courseId in studentDto.CourseIds)
            {
                var enrollment = new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = courseId
                };
                _context.Enrollments.Add(enrollment);
            }

            await _context.SaveChangesAsync();

            // Fetch full course details for the updated enrolled courses
            var enrolledCourses = await _context.Courses
                .Where(c => studentDto.CourseIds.Contains(c.Id))
                .ToListAsync();

            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Address = student.Address,
                Phone = student.Phone,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                Courses = enrolledCourses.Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };
        }


        // Delete a student by ID
        public async Task<bool> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ImportResult> ImportStudentsAsync(IFormFile file)
        {
            var result = new ImportResult();
            var students = new List<StudentImportDto>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(stream, config))
                {
                    csv.Context.RegisterClassMap<StudentImportMap>();  // Register the explicit mapping
                    students = csv.GetRecords<StudentImportDto>().ToList();
                }

                foreach (var studentData in students)
                {
                    try
                    {
                        var existingStudent = await _context.Students
                            .SingleOrDefaultAsync(s => s.Email == studentData.Email);

                        if (existingStudent == null)
                        {
                            var student = new Student
                            {
                                Name = studentData.Name,
                                Email = studentData.Email,
                                Phone = studentData.Phone,
                                Gender = studentData.Gender,
                                DateOfBirth = DateTime.ParseExact(studentData.DateOfBirth, "M/d/yyyy", CultureInfo.InvariantCulture),
                                Address = studentData.Address
                            };

                            _context.Students.Add(student);
                            result.SuccessCount++;
                        }
                        else
                        {
                            result.Errors.Add(new { student = studentData, message = "Email already exists" });
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new { student = studentData, message = "Error creating student", error = ex.Message });
                    }
                }

                await _context.SaveChangesAsync();
                result.ErrorCount = result.Errors.Count;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new { message = "Error processing CSV", error = ex.Message });
                result.ErrorCount++;
            }

            return result;
        }
    }
}
