using backend.Dtos;

namespace backend.Services
{
    public interface ITaskSubmissionService
    {
        // Create a new task submission
        Task<ServiceResult> CreateTaskSubmissionAsync(TaskSubmissionCreateDto dto);

        // Retrieve task submissions with optional search query
        Task<ServiceResult> GetTaskSubmissionsAsync(string searchQuery);

        // Retrieve a specific task submission by ID
        Task<ServiceResult> GetTaskSubmissionByIdAsync(int id);

        // Update a task submission
        Task<ServiceResult> UpdateTaskSubmissionAsync(int id, TaskSubmissionUpdateDto dto);

        // Delete a task submission
        Task<ServiceResult> DeleteTaskSubmissionAsync(int id);
    }
}
