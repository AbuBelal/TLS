using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    public class WReportDetailRepository : IWReportDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public WReportDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WReportDetail>> GetAllAsync()
        {
            return await _context.WReportDetails
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<WReportDetail?> GetByIdAsync(long id)
        {
            return await _context.WReportDetails
                .Include(d => d.WReport) // جلب بيانات التقرير الرئيسي المرتبط
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // جلب جميع التفاصيل التابعة لتقرير محدد
        public async Task<IEnumerable<WReportDetail>> GetByWReportIdAsync(long wReportId)
        {
            return await _context.WReportDetails
                .Where(d => d.WReportId == wReportId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<WReportDetail> AddAsync(WReportDetail reportDetail)
        {
            // ضمان تعيين تاريخ الإدخال إذا لم يكن مرسلاً
            reportDetail.SubmissionDate ??= DateTime.Now;

            await _context.WReportDetails.AddAsync(reportDetail);
            await _context.SaveChangesAsync();
            return reportDetail;
        }

        public async Task UpdateAsync(WReportDetail reportDetail)
        {
            _context.WReportDetails.Update(reportDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var reportDetail = await _context.WReportDetails.FindAsync(id);
            if (reportDetail != null)
            {
                _context.WReportDetails.Remove(reportDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
