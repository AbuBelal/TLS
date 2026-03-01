using SharedLib.Entities;
using APIServerLib.Repositories.Interfaces;
using SharedLib.Responses;
using Microsoft.EntityFrameworkCore;
using APIServerLib.Data;

namespace APIServerLib.Repositories.Implemntations
{
    public class EmpCenterRepository : IEmpCenterRepository
    {
        private readonly ApplicationDbContext _context;

        public EmpCenterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmpCenter>> GetAll()
        {
            return await _context.EmpCenters.ToListAsync();
        }

        public async Task<EmpCenter> GetById(long id)
        {
            return await _context.EmpCenters.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(EmpCenter item)
        {
            _context.EmpCenters.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "EmpCenter added successfully.");
        }

        public async Task<GeneralResponse> Update(EmpCenter item)
        {
            _context.EmpCenters.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "EmpCenter updated successfully.");
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var empCenter = await _context.EmpCenters.FindAsync(id);
            if (empCenter == null)
                return new GeneralResponse(false, "EmpCenter not found.", 0);

            _context.EmpCenters.Remove(empCenter);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "EmpCenter deleted successfully.", id);
        }
    }
}