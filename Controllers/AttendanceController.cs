using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/attendances")]
    [ApiController]
    public class AttendanceController : BaseController
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // Create a new attendance record
        [HttpPost("create")]
        public async Task<IActionResult> CreateAttendance([FromBody] AttendanceCreateDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Date) || string.IsNullOrEmpty(dto.Status) || dto.StudentId == 0 || dto.CourseId == 0)
                return BadRequest(new { message = "Missing required fields" });

            var result = await _attendanceService.CreateAttendanceAsync(dto);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // Retrieve all attendance records
        [HttpGet]
        public async Task<IActionResult> GetAttendanceRecords(
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? status = null,
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            var result = await _attendanceService.GetAttendanceRecordsAsync(searchQuery, status, startDate, endDate);
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }


        // Get a specific attendance record by ID
        [HttpGet("getOneAttendance/{id}")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            var result = await _attendanceService.GetAttendanceByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { message = "Attendance record not found" });
        }

        // Update attendance record
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceUpdateDto dto)
        {
            var result = await _attendanceService.UpdateAttendanceAsync(id, dto);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // Delete attendance record
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var result = await _attendanceService.DeleteAttendanceAsync(id);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }
    }
}
