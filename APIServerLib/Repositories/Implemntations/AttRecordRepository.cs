using SharedLib.Entities;
using APIServerLib.Repositories.Interfaces;
using SharedLib.Responses;
using Microsoft.EntityFrameworkCore;
using APIServerLib.Data;

namespace APIServerLib.Repositories.Implemntations
{
    public class AttRecordRepository : IAttRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public AttRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GenerateMonthlyAttendanceAsync(int year, int month, long holidayCode)
        {
            // 1. تحديد بداية ونهاية الشهر
            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);//

            int daysInMonth = DateTime.DaysInMonth(year, month);

            // 2. جلب جميع الموظفين مع مراكزهم الحالية لتجنب مشكلة N+1
            var employees = await _context.Employees
                .Include(e => e.EmpCenters)
                .ToListAsync();

            // 3. جلب السجلات الموجودة مسبقاً لهذا الشهر لتجنب التكرار
            var existingRecords = await _context.AttendanceRecord
                .Where(r => r.Date.Year == year && r.Date.Month == month)
                .Select(r => r.EmployeeId)
                .ToListAsync();
            //var existingRecords = await _context.AttendanceRecords
            //    .Where(r => r.Date == monthStart)
            //    .Select(r => r.EmployeeId)
            //    .ToListAsync();

            var newRecords = new List<AttendanceRecord>();

            foreach (var emp in employees)
            {
                // التحقق من التكرار
                if (existingRecords.Contains(emp.Id)) continue;

                // تحديد المركز الأكثر نشاطاً
                var activeCenterId = GetMostActiveCenterId(emp, monthStart, monthEnd);

                if(activeCenterId == null) continue;

                // إنشاء السجل الأساسي
                var record = new AttendanceRecord
                {
                    EmployeeId = emp.Id,
                    CenterId = activeCenterId,
                    Date = monthStart, // نعتبر تاريخ بداية الشهر هو تاريخ السجل
                    Comments = $"Generated for {month}/{year}"
                };

                // ملء أيام الشهر والتحقق من العطلة (الجمعة)
                FillDaysLogic(record, year, month, daysInMonth, holidayCode);

                newRecords.Add(record);
            }

            if (newRecords.Any())
            {
                try
                {
                await _context.AttendanceRecord.AddRangeAsync(newRecords);
                await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return newRecords.Count;
        }

        private long? GetMostActiveCenterId(Employee emp, DateOnly monthStart, DateOnly monthEnd)
        {
            // حساب تداخل الفترات: (المركز الأطول مدة داخل الشهر)
            return emp.EmpCenters
             .Where(c => c.FromDate <= monthEnd && (c.ToDate == null || c.ToDate >= monthStart))
             // التعديل هنا: نستخدم OrderByDescending لنجعل الـ true يظهر في بداية القائمة
             .OrderByDescending(c => c.IsActive)
             .ThenByDescending(c =>
             {
                 var start = c.FromDate > monthStart ? c.FromDate : monthStart;
                 var end = (c.ToDate == null || c.ToDate > monthEnd) ? monthEnd : c.ToDate.Value;
                 return end.DayNumber - start.DayNumber; // حساب عدد الأيام
             })
             .FirstOrDefault()?.CenterId;
        }

        private void FillDaysLogic(AttendanceRecord record, int year, int month, int daysInMonth, long holidayCode)
        {
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                bool isFriday = date.DayOfWeek == DayOfWeek.Friday;

                // إذا كانت جمعة، نضع كود العطلة
                if (isFriday)
                {
                    SetPropertyByDay(record, day, holidayCode);
                }
            }
        }

        private void SetPropertyByDay(AttendanceRecord record, int day, long holidayCode)
        {
            // بما أن الـ Model مصمم بـ Flattening (Day01, Day02...)
            // نستخدم Reflection أو Switch Statement لتحديث الحقل

            var propInfo = record.GetType().GetProperty($"Day{day:D2}_Desc");
            propInfo?.SetValue(record, holidayCode);
        }



        public async Task<List<AttendanceRecord>> GetAttendanceByCenterAsync(long centerId, int year, int month)
        {
            if (centerId == 0)
            {
                // جلب السجلات مع بيانات الموظف لعرض اسمه في الواجهة
                return await _context.AttendanceRecord
                    .Include(a => a.Employee)
                    .Include(c=>c.Center)
                    .Where(a => a.Date.Year == year && a.Date.Month == month)
                    .OrderBy(c=>c.Center.SortOrder)
                    .ToListAsync();
            }
            else
            {
                // جلب السجلات مع بيانات الموظف لعرض اسمه في الواجهة
                return await _context.AttendanceRecord
                    .Include(a => a.Employee)
                    //.Include(c => c.Center)
                    .Where(a => a.CenterId == centerId && a.Date.Year == year && a.Date.Month == month)
                    .ToListAsync();
            }
        }

        public async Task<bool> UpdateAttendanceRecordsAsync(List<AttendanceRecord>? updatedRecords)
        {
            foreach (var record in updatedRecords)
            {
                // البحث عن السجل الأصلي في قاعدة البيانات
                var existingRecord = await _context.AttendanceRecord
                    .FirstOrDefaultAsync(r => r.EmployeeId == record.EmployeeId && r.Date == record.Date);

                if (existingRecord != null)
                {
                    // هذه الميزة السحرية في EF Core ستقوم بنسخ جميع القيم (بما فيها الـ 31 يوماً)
                    // من الكائن القادم من الواجهة إلى الكائن الموجود في قاعدة البيانات
                    _context.Entry(existingRecord).CurrentValues.SetValues(record);
                }
            }

            // حفظ التعديلات دفعة واحدة
            await _context.SaveChangesAsync();
            return true;
        }
    }
}