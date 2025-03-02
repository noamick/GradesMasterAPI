using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new task
        public async Task<ServiceResult> CreateTaskAsync(TaskCreateDto dto)
        {
            var result = new ServiceResult();
            try
            {
                // Parse the date and TaskType enum
                if (!DateTime.TryParse(dto.Date, out var taskDate))
                {
                    result.Message = "Invalid date format";
                    return result;
                }

                if (!Enum.TryParse(dto.Type, true, out TaskType taskType))
                {
                    result.Message = "Invalid task type";
                    return result;
                }

                // Check if the CourseId exists in the database
                var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId);
                if (!courseExists)
                {
                    result.Message = "Invalid CourseId. The course does not exist.";
                    return result;
                }

                var task = new TaskDetail
                {
                    Name = dto.Name,
                    Type = taskType,
                    Date = taskDate.Date,  // Ensure only the date part is stored
                    CourseId = dto.CourseId,
                    Description = dto.Description
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = task;
                result.Message = "Task created successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error creating task: {ex.Message}";
            }
            return result;
        }


        // Retrieve tasks by search query
        public async Task<ServiceResult> GetTasksAsync(string searchQuery)
        {
            var result = new ServiceResult();
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => string.IsNullOrEmpty(searchQuery) || t.Name.Contains(searchQuery))
                    .ToListAsync();

                result.Success = true;
                result.Data = tasks;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching tasks: {ex.Message}";
            }
            return result;
        }

        // Retrieve a specific task by ID
        public async Task<ServiceResult> GetTaskByIdAsync(int id)
        {
            var result = new ServiceResult();
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.Course)
                    .Include(t => t.Submissions)
                    .ThenInclude(s => s.Student)
                    .SingleOrDefaultAsync(t => t.Id == id);

                if (task == null)
                {
                    result.Message = "Task not found";
                    return result;
                }

                // Map the task to TaskDto, converting TaskType enum to string
                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Name = task.Name,
                    Type = task.Type.ToString(),  // Convert TaskType enum to string
                    Date = task.Date,
                    Description = task.Description,
                    Course = new CourseDto
                    {
                        Id = task.Course.Id,
                        Name = task.Course.Name
                    },
                    Submissions = task.Submissions.Select(s => new TaskSubmissionDto
                    {
                        Id = s.Id,
                        StudentId = s.Student.Id,  // Assign StudentId
                        Student = new StudentDto
                        {
                            Id = s.Student.Id,
                            Name = s.Student.Name
                        },
                        Grade = s.Grade,
                        SubmissionDate = s.SubmissionDate
                    }).ToList()
                };

                result.Success = true;
                result.Data = taskDto;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching task: {ex.Message}";
            }
            return result;
        }


        public async Task<ServiceResult> UpdateTaskAsync(int id, TaskUpdateDto dto)
        {
            var result = new ServiceResult();
            try
            {
                // Use Include to load the related Course entity
                var task = await _context.Tasks
                    .Include(t => t.Course)  // Ensure the related Course is loaded
                    .SingleOrDefaultAsync(t => t.Id == id);

                if (task == null)
                {
                    result.Message = "Task not found";
                    return result;
                }

                // Parse and validate the date
                if (!DateTime.TryParse(dto.Date, out var taskDate))
                {
                    result.Message = "Invalid date format";
                    return result;
                }

                // Parse and validate the task type
                if (!Enum.TryParse(dto.Type, true, out TaskType taskType))
                {
                    result.Message = "Invalid task type";
                    return result;
                }

                // Update the task properties
                task.Name = dto.Name;
                task.Type = taskType;
                task.SetDateOnly(taskDate);
                task.CourseId = dto.CourseId;
                task.Description = dto.Description;

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Return the updated task as a DTO
                var updatedTaskDto = new TaskDto
                {
                    Id = task.Id,
                    Name = task.Name,
                    Type = task.Type.ToString(),  // Convert TaskType enum to string
                    Date = task.Date,
                    Course = task.Course != null
                        ? new CourseDto
                        {
                            Id = task.Course.Id,
                            Name = task.Course.Name
                        }
                        : null
                };

                result.Success = true;
                result.Data = updatedTaskDto;
                result.Message = "Task updated successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error updating task: {ex.Message}";
            }
            return result;
        }




        // Delete a task
        public async Task<ServiceResult> DeleteTaskAsync(int id)
        {
            var result = new ServiceResult();
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    result.Message = "Task not found";
                    return result;
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Task deleted successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error deleting task: {ex.Message}";
            }
            return result;
        }
    }
}
