using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/gradeWeights")]
    [ApiController]
    public class GradeWeightController : ControllerBase
    {
        private readonly IGradeWeightService _gradeWeightService;

        public GradeWeightController(IGradeWeightService gradeWeightService)
        {
            _gradeWeightService = gradeWeightService;
        }

        // Create a new grade weight
        [HttpPost("create")]
        public async Task<IActionResult> CreateGradeWeight([FromBody] GradeWeightCreateDto dto)
        {
            if (dto == null || dto.CourseId == 0 || string.IsNullOrEmpty(dto.ItemType) || dto.Weight <= 0)
                return BadRequest(new { message = "Missing or invalid required fields" });

            var result = await _gradeWeightService.CreateGradeWeightAsync(dto);
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }

        // Get all grade weights
        [HttpGet]
        public async Task<IActionResult> GetGradeWeights()
        {
            var result = await _gradeWeightService.GetAllGradeWeightsAsync();
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }

        // Get a specific grade weight by ID
        [HttpGet("getOneGradeWeight/{id}")]
        public async Task<IActionResult> GetGradeWeightById(int id)
        {
            var result = await _gradeWeightService.GetGradeWeightByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { message = "Grade weight not found" });
        }

        // Update an existing grade weight
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateGradeWeight(int id, [FromBody] GradeWeightUpdateDto dto)
        {
            var result = await _gradeWeightService.UpdateGradeWeightAsync(id, dto);
            return result.Success ? Ok(result.Data) : StatusCode(500, result);
        }

        // Delete a grade weight
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGradeWeight(int id)
        {
            var result = await _gradeWeightService.DeleteGradeWeightAsync(id);
            return result.Success ? Ok(result.Message) : StatusCode(500, result);
        }

        // Import grade weights from CSV
        [HttpPost("import")]
        public async Task<IActionResult> ImportGradeWeights([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Please upload a valid CSV file." });

            var result = await _gradeWeightService.ImportGradeWeightsAsync(file);

            if (result.ErrorCount > 0)
            {
                return Ok(new
                {
                    message = "Grade weights import process completed with errors",
                    successCount = result.SuccessCount,
                    errorCount = result.ErrorCount,
                    errors = result.Errors
                });
            }

            return Ok(new { message = "Grade weights imported successfully", successCount = result.SuccessCount });
        }
    }
}
