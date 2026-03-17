using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;
using Microsoft.AspNetCore.Identity;

namespace APIServerLib.Repositories.Implemntations
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRolesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<IdentityUserRole<string>>> GetAll()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<IdentityUserRole<string>> GetById(string id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(IdentityUserRole<string> item)
        {
            _context.UserRoles.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "User Role added successfully.");
        }

        public async Task<GeneralResponse> Update(IdentityUserRole<string> item)
        {
            _context.UserRoles.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "User Role updated successfully.");
        }

        public async Task<GeneralResponse> DeleteById(string id)
        {
            var userrole = await _context.UserRoles.FindAsync(id);
            if (userrole == null)
                return new GeneralResponse(false, "User Role not found.", 0);

            _context.UserRoles.Remove(userrole);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "User Role deleted successfully.");
        }

        Task<IdentityRole> IUserRolesRepository.GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUserRole<string>> GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> DeleteById(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<IdentityRole>> GetRolesForUser(long id)
        {
            var userroles =  await _context.UserRoles.Where(_=> _.UserId == id.ToString()).ToListAsync();
            var roles = await _context.Roles.Where(_ => userroles.Select(ur => ur.RoleId).Contains(_.Id)).ToListAsync();
            return roles;
        }
    }
}