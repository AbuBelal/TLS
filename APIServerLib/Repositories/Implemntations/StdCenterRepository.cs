using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    public class StdCenterRepository : IStdCenterRepository
    {
        private readonly ApplicationDbContext _context;

        public StdCenterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StdCenter>> GetAll()
        {
            return await _context.StdCenters.ToListAsync();
        }

        public async Task<StdCenter> GetById(long id)
        {
            return await _context.StdCenters.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(StdCenter item)
        {
            _context.StdCenters.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "StdCenter added successfully.");
        }

        public async Task<GeneralResponse> Update(StdCenter item)
        {
            _context.StdCenters.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "StdCenter updated successfully.");
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var stdCenter = await _context.StdCenters.FindAsync(id);
            if (stdCenter == null)
                return new GeneralResponse(false, "StdCenter not found.", 0);

            _context.StdCenters.Remove(stdCenter);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "StdCenter deleted successfully.");
        }
    }
}