using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    // Repositories/InComeRepository.cs
    public class InComeRepository : IInComeRepository
    {
        private readonly ApplicationDbContext _context;

        public InComeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InCome>> GetAllAsync()
        {
            return await _context.InComes
                .AsNoTracking()
                .Include(i => i.Center)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<InCome>> GetByCenterIdAsync(long centerId)
        {
            return await _context.InComes
                .AsNoTracking()
                .Where(i => i.CenterId == centerId)
                .Include(i => i.Center)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<InCome>> GetByDateRangeAsync(DateOnly from, DateOnly to)
        {
            return await _context.InComes
                .AsNoTracking()
                .Where(i => i.Date >= from && i.Date <= to)
                .Include(i => i.Center)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task<InCome?> GetByIdAsync(long id)
        {
            return await _context.InComes
                .Include(i => i.Center)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<InCome> CreateAsync(InCome income)
        {
            _context.InComes.Add(income);
            await _context.SaveChangesAsync();
            return income;
        }

        public async Task<InCome?> UpdateAsync(long id, InCome income)
        {
            var existing = await _context.InComes.FindAsync(id);
            if (existing == null) return null;

            existing.Name = income.Name;
            existing.Comments = income.Comments;
            existing.Date = income.Date;
            existing.Qnty = income.Qnty;
            existing.CenterId = income.CenterId;
            existing.RecipientName = income.RecipientName;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var income = await _context.InComes.FindAsync(id);
            if (income == null) return false;

            _context.InComes.Remove(income);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalAmountAsync(long? centerId = null)
        {
            var query = _context.InComes.AsQueryable();

            if (centerId.HasValue)
                query = query.Where(i => i.CenterId == centerId.Value);

            return await query.SumAsync(i => i.Qnty);
        }
        public async Task<decimal> GetBuildingTotalAmountAsync(string? BuildingId = null)
        {
            var query = _context.InComes.AsQueryable();

            if (BuildingId is not null)
                query = query.Where(i => i.Center.BuildingCode == BuildingId);

            return await query.SumAsync(i => i.Qnty);
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.InComes.AnyAsync(i => i.Id == id);
        }
    }
}
