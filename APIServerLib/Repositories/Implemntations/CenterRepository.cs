using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;

namespace SharedLib.Repositories
{
    public class CenterRepository : ICenterRepository
    {
        private readonly ApplicationDbContext _context;

        public CenterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Center>> GetAll()
        {
            return await _context.Centers.ToListAsync();
        }

        public async Task<Center> GetById(long id)
        {
            return await _context.Centers.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(Center item)
        {
            _context.Centers.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Center added successfully.", item.Id);
        }

        public async Task<GeneralResponse> Update(Center item)
        {
            _context.Centers.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Center updated successfully.", item.Id);
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var center = await _context.Centers.FindAsync(id);
            if (center == null)
                return new GeneralResponse(false, "Center not found.", 0);

            _context.Centers.Remove(center);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Center deleted successfully.", id);
        }
    }
}