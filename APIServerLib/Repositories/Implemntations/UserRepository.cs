using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    using APIServerLib.Data;
    using APIServerLib.Repositories.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using SharedLib.DTOs;
    using SharedLib.Entities;
    using SharedLib.Responses;

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> DeleteById(string Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
                return new GeneralResponse(false, "User not found.", 0);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "User deleted successfully.");
        }
        public async Task<GeneralResponse> DeleteById(long id)
        {
            return null;
        }

        public async Task<List<ApplicationUser>> GetAll()
        {
            return await _context.Users.Include(_=>_.Employee).ToListAsync();
        }
        public async Task<List<UserWithRoles?>> GetAllWithRols()
        {
            var users =  await _context.Users.Include(_=>_.Employee).ToListAsync();
            List<UserWithRoles> Userwithroles = new List<UserWithRoles>();
            return users.Select(user => new UserWithRoles
            {
                Email = user.Email,
                Employee = user.Employee,
                Id = user.Id,
                UserName = user.UserName,
                Roles = _context.UserRoles.Where(ur => ur.UserId == user.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToList()
            }).ToList();
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        public Task<ApplicationUser> GetById(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> GetUserByEmail(string Email)
        {
           
            return string.IsNullOrEmpty(Email) ? null : await _context.Users
                .Include(u => u.Employee).ThenInclude(x => x.EmpCenters).ThenInclude(x => x.Center)
                .Where(u => u.Email == Email)
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Employee).ThenInclude(x=>x.EmpCenters).ThenInclude(x=>x.Center)
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileDto
                {
                    UserId = u.Id,
                    Email = u.Email!,
                    EmployeeId = u.EmployeeId,
                    EmployeeName = u.Employee != null ? u.Employee.Name : "N/A",
                    CenterId = u.Employee.EmpCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault().CenterId,
                    CenterName = u.Employee.EmpCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault().Center.Name
                })
                .FirstOrDefaultAsync();
        }

        public Task<GeneralResponse> Insert(ApplicationUser item)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> Update(ApplicationUser item)
        {
            _context.Users.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "User updated successfully.");
        }
    }
}
