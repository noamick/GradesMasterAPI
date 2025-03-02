using backend.Dtos;

namespace backend.Services
{
    public interface ITaskService
    {
        Task<ServiceResult> CreateTaskAsync(TaskCreateDto dto);
        Task<ServiceResult> GetTasksAsync(string searchQuery);
        Task<ServiceResult> GetTaskByIdAsync(int id);
        Task<ServiceResult> UpdateTaskAsync(int id, TaskUpdateDto dto);
        Task<ServiceResult> DeleteTaskAsync(int id);
    }
}
