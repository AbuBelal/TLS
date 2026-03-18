using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAll()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> GetById(long id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(Student item)
        {
            _context.Students.Add(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Student added successfully.", item.Id);
        }

        public async Task<GeneralResponse> Update(Student item)
        {
            _context.Students.Update(item);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Student updated successfully.", item.Id);
        }

        public async Task<GeneralResponse> DeleteById(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return new GeneralResponse(false, "Student not found.", 0);

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Student deleted successfully.", id);
        }

        public async Task<int> GetCenterStudentsCountAsync(long CenterId)
        {
            //var count =await _context.Students.Where(s => s.StdCenters.Any(sc => sc.CenterId == CenteId && sc.ToDate == null)).CountAsync();
            var count =await _context.Students.Where(s => s.StdCenters.OrderByDescending(x=>x.FromDate).First().CenterId== CenterId).CountAsync();
            return count;
        }
    }
}