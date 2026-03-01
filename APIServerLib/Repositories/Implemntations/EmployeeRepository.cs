using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetById(long id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(Employee item)
        {
            _context.Employees.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse (  true,  "Employee added successfully." ,item.Id);
        }

        public async Task<GeneralResponse> Update(Employee item)
        {
            _context.Employees.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse( true,  "Employee updated successfully." );
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return new GeneralResponse (false,  "Employee not found." );

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return new GeneralResponse  ( true, "Employee deleted successfully." );
        }
    }
}