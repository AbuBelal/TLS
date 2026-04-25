using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    // Interfaces/IInComeRepository.cs
    public interface IInComeRepository
    {
        Task<IEnumerable<InCome>> GetAllAsync();
        Task<IEnumerable<InCome>> GetByCenterIdAsync(long centerId);
        Task<IEnumerable<InCome>> GetByDateRangeAsync(DateOnly from, DateOnly to);
        Task<InCome?> GetByIdAsync(long id);
        Task<InCome> CreateAsync(InCome income);
        Task<InCome?> UpdateAsync(long id, InCome income);
        Task<bool> DeleteAsync(long id);
        Task<decimal> GetTotalAmountAsync(long? centerId = null);
        Task<bool> ExistsAsync(long id);
        Task<decimal> GetBuildingTotalAmountAsync(string? BuildingId = null);
    }
}
