using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;
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
            // 1. تفريغ العلاقات لعدم قفل الشجرة المترابطة
            //reportDetail.WReport = null;
            // 1. البحث عن العنصر الموجود فعلياً في قاعدة البيانات (Tracked Entity)
            var existingDetail = await _context.WReportDetails.FindAsync(reportDetail.Id);

            if (existingDetail != null)
            {
                // 2. نسخ جميع القيم الجديدة إلى الكائن الحالي المتتبع تلقائياً
                _context.Entry(existingDetail).CurrentValues.SetValues(reportDetail);

                // 3. ضمان عدم ضياع المفتاح الأجنبي WReportId إذا جاء فارغاً من الواجهة
                if (reportDetail.WReportId == null)
                {
                    _context.Entry(existingDetail).Property(x => x.WReportId).IsModified = false;
                }

                // 4. إرسال أمر UPDATE إلى قاعدة البيانات
                await _context.SaveChangesAsync();
            }

            //// 2. فحص وإلغاء أي تتبع قديم لنفس الكائن في الـ Change Tracker
            //var local = _context.WReportDetails
            //    .Local
            //    .FirstOrDefault(entry => entry.Id == reportDetail.Id);

            //if (local != null)
            //{
            //    _context.Entry(local).State = EntityState.Detached;
            //}

            //// 3. تعيين حالة الكائن إلى Modified
            //_context.Entry(reportDetail).State = EntityState.Modified;

            //// 4. الحفظ مع تعيين مهلة واستخدام ConfigureAwait لتجنب الـ Deadlock
            //_context.Database.SetCommandTimeout(15); // تحديد مهلة 15 ثانية للشفافية

            //try
            //{
            //    await _context.SaveChangesAsync().ConfigureAwait(false);
            //}
            //catch (Exception ex)
            //{
            //    // إذا حدث خطأ أو مهلة، ستكتشفه هنا بدلاً من التجمّد
            //    throw new Exception($"خطأ أثناء الحفظ: {ex.Message}", ex);
            //}

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
