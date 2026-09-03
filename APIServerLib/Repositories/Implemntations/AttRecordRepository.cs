using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Reflection;
using System.Runtime.CompilerServices;
using TLSClientSharedLib.ViewModels;

namespace APIServerLib.Repositories.Implemntations
{
    public class AttRecordRepository : IAttRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public AttRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GenerateMonthlyAttendanceAsync(int year, int month, long? holidayCode)
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

        private void FillDaysLogic(AttendanceRecord record, int year, int month, int daysInMonth, long? holidayCode)
        {
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                bool isFriday = date.DayOfWeek == DayOfWeek.Friday;

                // إذا كانت جمعة، نضع كود العطلة
                if (isFriday)
                {
                    SetPropertyByDay(record, day, holidayCode??0);
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
                return  await _context.AttendanceRecord
                    .Include(a => a.Employee)
                    .Include(c=>c.Center)
                    .Where(a => a.Date.Year == year && a.Date.Month == month)
                    .OrderBy(c=>c.Center.SortOrder)
                    //.Select(r=> (new AttRecMappers()).ToDTO(r))
                    .ToListAsync();
            }
            else
            {
                // جلب السجلات مع بيانات الموظف لعرض اسمه في الواجهة
                return await _context.AttendanceRecord
                    .Include(a => a.Employee)
                    //.Include(c => c.Center)
                    .Where(a => a.CenterId == centerId && a.Date.Year == year && a.Date.Month == month)
                    //.Select(r => (new AttRecMappers()).ToDTO(r))
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

        public async Task LockAttendanceRecordsAsync(int year, int month, bool Lock=true)
        {
            //var recordsToLock = await _context.AttendanceRecord
            //    .Where(r => r.Date.Year == year && r.Date.Month == month && !(r.IsLocked??false))
            //    .ToListAsync();
            //foreach (var record in recordsToLock)
            //{
            //    record.IsLocked = true;
            //}
            //await _context.SaveChangesAsync();


            await _context.AttendanceRecord
              .Where(r => r.Date == new DateOnly(year, month, 1) && r.IsLocked !=Lock)
              .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsLocked, Lock));
        }
        public async Task<GeneralResponse> DeleteEmployeeAttendanceRecordsAsync(AttRecoRequest request)
        {
            var recordsToDelete = await _context.AttendanceRecord.Include(a=>a.Employee)
                .Where(r => r.EmployeeId == request.EmployeeId && r.Date.Month==request.Month && r.Date.Year==request.Year && r.CenterId==request.CenterId)
                .ToListAsync();

            if (!recordsToDelete.Any())
            {
                return new GeneralResponse
                (
                     false,
                     "لا يوجد سجل يمكن حذفه."
                );
            }
            _context.AttendanceRecord.RemoveRange(recordsToDelete);
            await _context.SaveChangesAsync();

            return new GeneralResponse
            (
                 true,
                 $"تم حذف سجل الحضور بنجاح للموظف/ {recordsToDelete.First().Employee?.Name} "
            );
        }

        // في أعلى الكلاس، قم بتخزين الخصائص مرة واحدة فقط في الذاكرة
        private static readonly System.Reflection.PropertyInfo[] AttendantProps = Enumerable.Range(1, 31)
            .Select(i => typeof(AttendanceRecordDto).GetProperty($"Day{i:D2}_IsAttendant"))
        .ToArray();

        private static readonly System.Reflection.PropertyInfo[] DescProps = Enumerable.Range(1, 31)
            .Select(i => typeof(AttendanceRecordDto).GetProperty($"Day{i:D2}_Desc"))
        .ToArray();

        public async Task<List<AttRecSummery>> GetAttendanceSummaryAsync(int year, int month)
        {
            int DaysInMonth = DateTime.DaysInMonth(year, month);
            var AllAttRec =await GetAttendanceByCenterAsync(0, year, month);
            var Dto = AllAttRec.Select(r => (new AttRecMappers()).ToDTO(r)).ToList();
            var EmployeeList = Dto.Select(r => MapToViewModel(r, DaysInMonth)).ToList();
            List<AttRecSummery> summaries = new List<AttRecSummery>();
            var Centers = await _context.Centers.ToListAsync();
            foreach (var center in Centers)
            {
                AttRecSummery Summery = new AttRecSummery();
                Summery.CenterId = center.Id;
                Summery.CenterId = center.Id;
                Summery.CenterId = center.Id;
                Summery.CenterId = center.Id;
                Summery.CenterId = center.Id;

                summaries.Add(new AttRecSummery
                {
                    CenterId = center.Id,
                    CenterName = center.Name,
                    Date = new DateOnly(year, month, 1),
                    TotalEmployees = EmployeeList.Count(e => e.OriginalRecord.CenterId == center.Id),
                    AttRecCompleted = EmployeeList.Count(e => e.OriginalRecord.CenterId == center.Id && e.EmptyDaysCount==0),
                    //AttRecPartialCompleted = EmployeeList.Count(e => e.OriginalRecord.CenterId == center.Id && e.EmptyDaysCount>0)
                });

            }


            return summaries;
        }
        private  int CountEnterdDays(AttendanceRecord Rec)
        {
            int EnteredDaye = 0;

            if (Rec == null) return EnteredDaye;

            PropertyInfo[] properties = Rec.GetType().GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                if (prop.Name.StartsWith("Day") && prop.Name.EndsWith("_IsAttendant") && prop.PropertyType == typeof(bool?))
                {
                    object? value = prop.GetValue(Rec);

                    if (value != null && (bool)value == true)
                    {
                        EnteredDaye++;
                    }
                }
                else
                if (prop.Name.StartsWith("Day") && prop.Name.EndsWith("_Desc") && prop.PropertyType == typeof(long?))
                {
                    object? value = prop.GetValue(Rec);

                    if (value != null && (long)value>0)
                    {
                        EnteredDaye++;
                    }
                }
            }

            return EnteredDaye;
        }

        private EmployeeAttendanceVM MapToViewModel(AttendanceRecordDto record, int daysInMonth)
        {

            var vm = new EmployeeAttendanceVM
            {

                EmployeeId = record.EmployeeId,
                EmployeeName = record.EmployeeName,
                CenterName = record.CenterName,
                OriginalRecord = (new AttRecMappers()).ToEntity(record),
            };

            // record.Center = null;
            // record.Employee = null;

            var type = typeof(AttendanceRecord);
            for (int i = 1; i <= daysInMonth; i++)
            {
                // var isAttendantProp = type.GetProperty($"Day{i:D2}_IsAttendant");
                // var descProp = type.GetProperty($"Day{i:D2}_Desc");

                vm.Days.Add(new DayVM
                {
                    DayNumber = i,
                    IsAttendant = AttendantProps[i - 1]?.GetValue(record) as bool?,
                    DescId = DescProps[i - 1]?.GetValue(record) as long?,
                    IsLocked = record.IsLocked ?? false
                });
            }
            return vm;
        }
    }
}