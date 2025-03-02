using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // Endpoint to generate a performance report for all students
        [HttpGet]
        public async Task<IActionResult> GetStudentPerformanceReports()
        {
            var result = await _reportService.GetStudentPerformanceReportsAsync();
            if (!result.Success)
            {
                return StatusCode(500, result.Message);
            }

            return Ok(result.Data);
        }
    }
}
