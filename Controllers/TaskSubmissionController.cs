using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/taskSubmissions")]
    [ApiController]
    public class TaskSubmissionController : ControllerBase
    {
        private readonly ITaskSubmissionService _taskSubmissionService;

        public TaskSubmissionController(ITaskSubmissionService taskSubmissionService)
        {
            _taskSubmissionService = taskSubmissionService;
        }

        // Create a new task submission
        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskSubmission([FromBody] TaskSubmissionCreateDto dto)
        {
            if (dto == null || dto.StudentId == 0 || dto.TaskId == 0 || dto.SubmissionDate == null)
                return BadRequest(new { message = "Missing required fields" });

            var result = await _taskSubmissionService.CreateTaskSubmissionAsync(dto);
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }

        // Get task submissions with optional search query
        [HttpGet]
        public async Task<IActionResult> GetTaskSubmissions([FromQuery] string? q = "")
        {
            var result = await _taskSubmissionService.GetTaskSubmissionsAsync(q);
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }

        // Retrieve a specific task submission by ID
        [HttpGet("getOneTaskSubmission/{id}")]
        public async Task<IActionResult> GetTaskSubmissionById(int id)
        {
            var result = await _taskSubmissionService.GetTaskSubmissionByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { message = "Task submission not found" });
        }

        // Update a task submission
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTaskSubmission(int id, [FromBody] TaskSubmissionUpdateDto dto)
        {
            var result = await _taskSubmissionService.UpdateTaskSubmissionAsync(id, dto);
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }

        // Delete a task submission
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTaskSubmission(int id)
        {
            var result = await _taskSubmissionService.DeleteTaskSubmissionAsync(id);
            return result.Success ? Ok(result.Message) : StatusCode(500, result);
        }
    }
}
