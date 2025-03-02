using backend.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IGradeWeightService
    {
        Task<ServiceResult> CreateGradeWeightAsync(GradeWeightCreateDto dto);

        Task<ServiceResult> GetAllGradeWeightsAsync();

        Task<ServiceResult> GetGradeWeightByIdAsync(int id);

        Task<ServiceResult> UpdateGradeWeightAsync(int id, GradeWeightUpdateDto dto);

        Task<ServiceResult> DeleteGradeWeightAsync(int id);

        Task<ImportResult> ImportGradeWeightsAsync(IFormFile file);
    }
}
