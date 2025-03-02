using backend.Dtos;

namespace backend.Services
{
    public interface IAttendanceService
    {
        Task<ServiceResult> CreateAttendanceAsync(AttendanceCreateDto dto);
        Task<ServiceResult> GetAttendanceRecordsAsync(string searchQuery, string status, string startDate, string endDate);
        Task<ServiceResult> GetAttendanceByIdAsync(int id);
        Task<ServiceResult> UpdateAttendanceAsync(int id, AttendanceUpdateDto dto);
        Task<ServiceResult> DeleteAttendanceAsync(int id);
    }
}
