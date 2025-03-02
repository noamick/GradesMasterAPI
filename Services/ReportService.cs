using backend.Data;
using backend.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceReportResult<List<StudentReportDto>>> GetStudentPerformanceReportsAsync()
        {
            var result = new ServiceReportResult<List<StudentReportDto>>();
            try
            {
                // Fetch data for students, enrollments, associated entities, and attendance
                var students = await _context.Students
                    .Include(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Tasks)
                        .ThenInclude(t => t.Submissions)
                    .Include(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.GradeWeights)
                    .Include(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Attendances) // Include attendance records
                    .Select(s => new StudentReportDto
                    {
                        StudentId = s.Id,
                        StudentName = s.Name,
                        Courses = s.Enrollments.Select(e => new CourseReportDto
                        {
                            CourseName = e.Course.Name,
                            Description = e.Course.Description,
                            TaskSubmissions = e.Course.Tasks
                                .SelectMany(t => t.Submissions
                                    .Where(sub => sub.StudentId == s.Id))
                                .ToList(),
                            GradeWeights = e.Course.GradeWeights.ToList(),
                            AttendanceRecords = e.Course.Attendances
                                .Where(a => a.StudentId == s.Id) // Filter attendance records for this student
                                .ToList(),
                        }).ToList()
                    })
                    .ToListAsync();

                // Check for empty data
                if (students == null || students.Count == 0)
                {
                    result.Message = "No students found.";
                    result.Success = true;
                    return result;
                }

                result.Success = true;
                result.Data = students;
            }
            catch (Exception ex)
            {
                result.Message = $"Error generating report: {ex.Message}";
                result.Success = false;
            }

            return result;
        }



    }
}
