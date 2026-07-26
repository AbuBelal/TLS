using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    public class WReportRepository : IWReportRepository
    {
        private readonly ApplicationDbContext _context;

        public WReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WReport>> GetAllAsync()
        {
            // قد ترغب في إضافة Pagination هنا لاحقاً إذا كانت البيانات كبيرة
            return await _context.WReports
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<WReport>> GetReportsByCenterIdAsync(long CurCenterId)
        {

            return await _context.WReports.Include(r => r.WReportDetails.Where(c=>c.CenterId==CurCenterId)) // جلب تفاصيل التقرير
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<WReport?> GetByIdAsync(long id)
        {
            return await _context.WReports
                .Include(r => r.WReportDetails) // جلب تفاصيل التقرير
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<WReport> AddAsync(WReport report)
        {
            
            await _context.WReports.AddAsync(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task UpdateAsync(WReport report)
        {
            _context.WReports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var report = await _context.WReports.FindAsync(id);
            if (report != null)
            {
                _context.WReports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }
    }
}