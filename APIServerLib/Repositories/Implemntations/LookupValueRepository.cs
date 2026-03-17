using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    public class LookupValueRepository : ILookupValueRepository
    {
        private readonly ApplicationDbContext _context;

        public LookupValueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LookupValue>> GetAll()
        {
            return await _context.LookupValues.ToListAsync();
        }

        public async Task<LookupValue> GetById(long id)
        {
            return await _context.LookupValues.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(LookupValue item)
        {
            _context.LookupValues.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "LookupValue added successfully.", item.Id);
        }

        public async Task<GeneralResponse> Update(LookupValue item)
        {
            _context.LookupValues.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "LookupValue updated successfully.", item.Id);
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var lookupValue = await _context.LookupValues.FindAsync(id);
            if (lookupValue == null)
                return new GeneralResponse(false, "LookupValue not found.", 0);

            _context.LookupValues.Remove(lookupValue);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "LookupValue deleted successfully.", id);
        }

        public async Task<List<LookupValue>> GetByLookupType(string ValueType)
        {
            return await _context.LookupValues.Where(_=>_.ValueType == ValueType).ToListAsync();
        }
    }
}