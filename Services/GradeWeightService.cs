using backend.Data;
using backend.Dtos;
using backend.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using backend.Mappings;

namespace backend.Services
{
    public class GradeWeightService : IGradeWeightService
    {
        private readonly ApplicationDbContext _context;

        public GradeWeightService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new grade weight
        public async Task<ServiceResult> CreateGradeWeightAsync(GradeWeightCreateDto dto)
        {
            var result = new ServiceResult();
            try
            {
                var gradeWeight = new GradeWeight
                {
                    ItemType = dto.ItemType,
                    Weight = dto.Weight,
                    CourseId = dto.CourseId
                };

                _context.GradeWeights.Add(gradeWeight);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = gradeWeight;
                result.Message = "Grade weight created successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error creating grade weight: {ex.Message}";
            }

            return result;
        }

        // Get all grade weights
        public async Task<ServiceResult> GetAllGradeWeightsAsync()
        {
            var result = new ServiceResult();
            try
            {
                var gradeWeights = await _context.GradeWeights
                    .Include(g => g.Course)
                    .Select(g => new
                    {
                        g.Id,
                        g.ItemType,
                        g.Weight,
                        Course = new { g.Course.Id, g.Course.Name, g.Course.Description }
                    })
                    .ToListAsync();

                result.Success = true;
                result.Data = gradeWeights;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching grade weights: {ex.Message}";
            }

            return result;
        }

        // Get a specific grade weight by ID
        public async Task<ServiceResult> GetGradeWeightByIdAsync(int id)
        {
            var result = new ServiceResult();
            try
            {
                var gradeWeight = await _context.GradeWeights
                    .Include(g => g.Course)
                    .SingleOrDefaultAsync(g => g.Id == id);

                if (gradeWeight == null)
                {
                    result.Message = "Grade weight not found";
                    return result;
                }

                result.Success = true;
                result.Data = gradeWeight;
            }
            catch (Exception ex)
            {
                result.Message = $"Error fetching grade weight: {ex.Message}";
            }

            return result;
        }

        // Update a grade weight
        public async Task<ServiceResult> UpdateGradeWeightAsync(int id, GradeWeightUpdateDto dto)
        {
            var result = new ServiceResult();
            try
            {
                var gradeWeight = await _context.GradeWeights.FindAsync(id);
                if (gradeWeight == null)
                {
                    result.Message = "Grade weight not found";
                    return result;
                }

                gradeWeight.ItemType = dto.ItemType;
                gradeWeight.Weight = dto.Weight;
                gradeWeight.CourseId = dto.CourseId;

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = gradeWeight;
                result.Message = "Grade weight updated successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error updating grade weight: {ex.Message}";
            }

            return result;
        }

        // Delete a grade weight
        public async Task<ServiceResult> DeleteGradeWeightAsync(int id)
        {
            var result = new ServiceResult();
            try
            {
                var gradeWeight = await _context.GradeWeights.FindAsync(id);
                if (gradeWeight == null)
                {
                    result.Message = "Grade weight not found";
                    return result;
                }

                _context.GradeWeights.Remove(gradeWeight);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Grade weight deleted successfully";
            }
            catch (Exception ex)
            {
                result.Message = $"Error deleting grade weight: {ex.Message}";
            }

            return result;
        }

        // Import grade weights from CSV
        public async Task<ImportResult> ImportGradeWeightsAsync(IFormFile file)
        {
            var result = new ImportResult();
            var gradeWeights = new List<GradeWeightImportDto>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(stream, config))
                {
                    csv.Context.RegisterClassMap<GradeWeightImportMap>(); // Explicit mapping
                    gradeWeights = csv.GetRecords<GradeWeightImportDto>().ToList();
                }

                foreach (var gradeWeightData in gradeWeights)
                {
                    try
                    {
                        // Check if a GradeWeight with the same CourseId and ItemType already exists
                        var existingGradeWeight = await _context.GradeWeights
                            .FirstOrDefaultAsync(gw => gw.CourseId == gradeWeightData.CourseId && gw.ItemType == gradeWeightData.ItemType);

                        if (existingGradeWeight != null)
                        {
                            continue; // Skip to the next record
                        }

                        var gradeWeight = new GradeWeight
                        {
                            ItemType = gradeWeightData.ItemType,
                            Weight = gradeWeightData.Weight,
                            CourseId = gradeWeightData.CourseId
                        };

                        _context.GradeWeights.Add(gradeWeight);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new { gradeWeight = gradeWeightData, message = ex.Message });
                    }
                }

                await _context.SaveChangesAsync();
                result.ErrorCount = result.Errors.Count;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new { message = "Error processing CSV", error = ex.Message });
                result.ErrorCount++;
            }

            return result;
        }

    }
}
