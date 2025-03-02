using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TaskSubmissionService : ITaskSubmissionService
    {
        private readonly ApplicationDbContext _context;

        public TaskSubmissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new task submission
        public async Task<ServiceResult> CreateTaskSubmissionAsync(TaskSubmissionCreateDto dto)
        {
            var result = new ServiceResult();
            try
            {
                var taskSubmission = new TaskSubmission
                {
                    Grade = dto.Grade,
                    SubmissionDate = DateTime.Parse(dto.SubmissionDate),
                    StudentId = dto.StudentId,
                    TaskId = dto.TaskId,
                    Feedback = dto.Feedback
                };

                _context.TaskSubmissions.Add(taskSubmission);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = taskSubmission;
                result.Message = "Task submission recorded successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error recording task submission: {ex.Message}";
            }

            return result;
        }

        // Get task submissions with optional search query
        public async Task<ServiceResult> GetTaskSubmissionsAsync(string searchQuery)
        {
            var result = new ServiceResult();
            try
            {
                var submissions = await _context.TaskSubmissions
                    .Include(ts => ts.Student)
                    .Include(ts => ts.Task)
                    .Where(ts => string.IsNullOrEmpty(searchQuery) || ts.Task.Name.Contains(searchQuery))
                    .Select(ts => new
                    {
                        ts.Id,
                        ts.Grade,
                        ts.SubmissionDate,
                        Student = new { ts.Student.Id, ts.Student.Name, ts.Student.Email },
                        Task = new { ts.Task.Id, ts.Task.Name, ts.Task.Type, ts.Task.Date }
                    })
                    .ToListAsync();

                result.Success = true;
                result.Data = submissions;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching submissions: {ex.Message}";
            }

            return result;
        }

        // Retrieve a specific task submission by ID
        public async Task<ServiceResult> GetTaskSubmissionByIdAsync(int id)
        {
            var result = new ServiceResult();
            try
            {
                var submission = await _context.TaskSubmissions.FindAsync(id);
                if (submission == null)
                {
                    result.Message = "Task submission not found";
                    return result;
                }

                result.Success = true;
                result.Data = submission;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching task submission: {ex.Message}";
            }

            return result;
        }

        // Update task submission
        public async Task<ServiceResult> UpdateTaskSubmissionAsync(int id, TaskSubmissionUpdateDto dto)
        {
            var result = new ServiceResult();
            try
            {
                var submission = await _context.TaskSubmissions.FindAsync(id);
                if (submission == null)
                {
                    result.Message = "Task submission not found";
                    return result;
                }

                submission.Grade = dto.Grade;
                submission.SubmissionDate = DateTime.Parse(dto.SubmissionDate);
                submission.StudentId = dto.StudentId;
                submission.TaskId = dto.TaskId;
                submission.Feedback = dto.Feedback;

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Task submission updated successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error updating task submission: {ex.Message}";
            }

            return result;
        }

        // Delete task submission
        public async Task<ServiceResult> DeleteTaskSubmissionAsync(int id)
        {
            var result = new ServiceResult();
            try
            {
                var submission = await _context.TaskSubmissions.FindAsync(id);
                if (submission == null)
                {
                    result.Message = "Task submission not found";
                    return result;
                }

                _context.TaskSubmissions.Remove(submission);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Task submission deleted successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error deleting task submission: {ex.Message}";
            }

            return result;
        }
    }
}
