using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : BaseController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // Create a new task
        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
        {
            var result = await _taskService.CreateTaskAsync(dto);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // Retrieve tasks by search query
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? q = "")
        {
            var result = await _taskService.GetTasksAsync(q);
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }

        // Retrieve a specific task by ID
        [HttpGet("getOneTask/{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        // Update a task
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto dto)
        {
            var result = await _taskService.UpdateTaskAsync(id, dto);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        // Delete a task
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }
    }
}
