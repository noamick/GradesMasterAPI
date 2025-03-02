using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] string? searchQuery = null)
        {
            var students = await _studentService.GetAllStudents(searchQuery);
            return Ok(students);
        }

        [HttpGet("getOneStudent/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentById(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto studentDto)
        {
            var result = await _studentService.CreateStudent(studentDto);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentUpdateDto studentDto)
        {
            var result = await _studentService.UpdateStudent(id, studentDto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudent(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportStudents([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Please upload a valid CSV file." });

            var result = await _studentService.ImportStudentsAsync(file);

            if (result.ErrorCount > 0)
            {
                return Ok(new
                {
                    message = "Students import process completed with errors",
                    successCount = result.SuccessCount,
                    errorCount = result.ErrorCount,
                    errors = result.Errors
                });
            }

            return Ok(new { message = "Students imported successfully", successCount = result.SuccessCount });
        }
    }
}
