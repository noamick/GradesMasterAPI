using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create attendance
        public async Task<ServiceResult> CreateAttendanceAsync(AttendanceCreateDto dto)
        {
            var result = new ServiceResult();

            try
            {
                // Validate required fields
                if (dto.StudentId == 0 || dto.CourseId == 0 || string.IsNullOrEmpty(dto.Status) || string.IsNullOrEmpty(dto.Date))
                {
                    result.Message = "Missing required fields.";
                    return result;
                }

                // Parse the Date
                if (!DateTime.TryParse(dto.Date, out var attendanceDate))
                {
                    result.Message = "Invalid date format.";
                    return result;
                }

                // Parse the Status Enum
                if (!Enum.TryParse(dto.Status, true, out AttendanceStatus status))
                {
                    result.Message = "Invalid attendance status.";
                    return result;
                }

                // Check if the student and course exist
                var studentExists = await _context.Students.AnyAsync(s => s.Id == dto.StudentId);
                var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId);
                if (!studentExists || !courseExists)
                {
                    result.Message = "Student or Course not found.";
                    return result;
                }

                // Create attendance record
                var attendance = new Attendance
                {
                    Date = attendanceDate,
                    Status = status,
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId
                };

                // Add attendance to the context
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = attendance;
                result.Message = "Attendance recorded successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error recording attendance: {ex.Message}";
            }

            return result;
        }

        // Get all attendance records
        public async Task<ServiceResult> GetAttendanceRecordsAsync(string searchQuery, string status, string startDate, string endDate)
        {
            var result = new ServiceResult();

            try
            {
                var query = _context.Attendances.AsQueryable();

                // Apply date range filter
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    if (DateTime.TryParse(startDate, out var start) && DateTime.TryParse(endDate, out var end))
                    {
                        query = query.Where(a => a.Date >= start && a.Date <= end);
                    }
                    else
                    {
                        result.Message = "Invalid date range.";
                        return result;
                    }
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<AttendanceStatus>(status, true, out var statusEnum))
                {
                    query = query.Where(a => a.Status == statusEnum);
                }

                // Apply search filter (student name)
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Include(a => a.Student)
                                 .Where(a => a.Student.Name.Contains(searchQuery));
                }

                // Get attendance records with student and course details
                var attendanceRecords = await query
                    .Include(a => a.Course)
                    .Select(a => new
                    {
                        a.Id,
                        a.Date,
                        a.Status,
                        Student = new { a.Student.Id, a.Student.Name, a.Student.Email },
                        Course = new { a.Course.Id, a.Course.Name, a.Course.Description }
                    })
                    .ToListAsync();

                result.Success = true;
                result.Data = attendanceRecords;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching attendance records: {ex.Message}";
            }

            return result;
        }

        // Get attendance by ID
        public async Task<ServiceResult> GetAttendanceByIdAsync(int id)
        {
            var result = new ServiceResult();

            try
            {
                var attendance = await _context.Attendances
                    .AsNoTracking()
                    .Include(a => a.Student)
                    .Include(a => a.Course)
                    .SingleOrDefaultAsync(a => a.Id == id);

                if (attendance == null)
                {
                    result.Message = "Attendance record not found.";
                    return result;
                }

                result.Success = true;
                result.Data = attendance;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching attendance record: {ex.Message}";
            }

            return result;
        }

        // Update attendance
        public async Task<ServiceResult> UpdateAttendanceAsync(int id, AttendanceUpdateDto dto)
        {
            var result = new ServiceResult();

            try
            {
                var attendance = await _context.Attendances.FindAsync(id);
                if (attendance == null)
                {
                    result.Message = "Attendance record not found.";
                    return result;
                }

                // Validate and update fields
                if (!DateTime.TryParse(dto.Date, out var date))
                {
                    result.Message = "Invalid date format.";
                    return result;
                }

                if (!Enum.TryParse(dto.Status, true, out AttendanceStatus status))
                {
                    result.Message = "Invalid attendance status.";
                    return result;
                }

                // Update attendance
                attendance.StudentId = dto.StudentId;
                attendance.CourseId = dto.CourseId;
                attendance.Date = date;
                attendance.Status = status;

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Attendance updated successfully.";
            }
            catch (Exception ex)
            {
                result.Message = $"Error updating attendance: {ex.Message}";
            }

            return result;
        }

        // Delete attendance
        public async Task<ServiceResult> DeleteAttendanceAsync(int id)
        {
            var result = new ServiceResult();

            try
            {
                var attendance = await _context.Attendances.FindAsync(id);
                if (attendance == null)
                {
                    result.Message = "Attendance record not found.";
                    return result;
                }

                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Attendance deleted successfully.";
            }
            catch (Exception ex)
            {
                result.Message = $"Error deleting attendance: {ex.Message}";
            }

            return result;
        }
    }
}
