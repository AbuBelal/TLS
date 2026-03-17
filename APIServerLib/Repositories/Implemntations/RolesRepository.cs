using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;
using Microsoft.AspNetCore.Identity;

namespace APIServerLib.Repositories.Implemntations
{
    public class RolesRepository : IRolesRepository
    {
        private readonly ApplicationDbContext _context;

        public RolesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<IdentityRole>> GetAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<IdentityRole> GetById(string id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(IdentityRole item)
        {
            _context.Roles.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Role added successfully.");
        }

        public async Task<GeneralResponse> Update(IdentityRole item)
        {
            _context.Roles.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Role updated successfully.");
        }

        public async Task<GeneralResponse> DeleteById(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return new GeneralResponse(false, "Role not found.", 0);

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Role deleted successfully.");
        }

        public Task<IdentityRole> GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> DeleteById(long id)
        {
            throw new NotImplementedException();
        }
    }
}