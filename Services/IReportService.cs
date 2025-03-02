using backend.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IReportService
    {
        // Method to generate performance reports for all students
        Task<ServiceReportResult<List<StudentReportDto>>> GetStudentPerformanceReportsAsync();
    }
}
