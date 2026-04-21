using APIServerLib.Data;
using APIServerLib.Data.Migrations;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using SharedLib.Responses;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    public class DailyReportRepository : IDailyReportRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string userId;
        public DailyReportRepository(ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor,
            IUserRepository UserRepository,
            IEmployeeRepository EmployeeRepository
            )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = UserRepository;
            _employeeRepository = EmployeeRepository;
            ///
            var httpContext = _httpContextAccessor.HttpContext;
            userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userName = httpContext?.User?.FindFirstValue(ClaimTypes.Name)
            //               ?? httpContext?.User?.Identity?.Name;
        }

        #region CurUser CurEmp Details
        private async Task<ApplicationUser> CurrentUser()
        {
            return await _userRepository.GetById(userId);
        }
        private async Task<Employee> CurrentEmployee()
        {
            var CurUser = await CurrentUser();
            return await _employeeRepository.GetById(CurUser.EmployeeId ?? 0);
        }
        private async Task<long> CurrentCenterId()
        {
            var Employee = await CurrentEmployee();
            return
                Employee is null ? 0 :
                Employee.EmpCenters
                .OrderByDescending(ec => ec.FromDate)
                .FirstOrDefault()?
                .CenterId ?? 0;
        }
        private async Task<string> CurUserRole()
        {
            var roleId = _context.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
            var role = _context.Roles.FirstOrDefault(x=>x.Id==roleId);
            return role.Name??string.Empty;
        }
        #endregion

        public async Task<GeneralResponse> DeleteDailyReportAsync(long Id)
        {
            var result =await  _context.DailyReports.FindAsync(Id);
            if (result != null)
            {
                _context.Remove(result);
                return new GeneralResponse(true,"تم حذف التقرير اليومي بنجاح",result.Id);
            }
            return new GeneralResponse(false, "لم يتم العثور على التقرير اليومي", 0);
        }

        public async Task<List<SharedLib.Entities.DailyReport>> GetDailyReportForDateAsync(DateOnly date)
        {

            string userrole = await CurUserRole();
            List<DailyReport> DailyReportList = new List<DailyReport>();

            var Centers = await _context.Centers.AsNoTracking().ToListAsync();

            // جلب جميع الطلاب النشطين مع البيانات المطلوبة
            var allStdCenters = await _context.StdCenters
                .AsNoTracking()
                .Where(sc => sc.ToDate == null)
                .Include(sc => sc.Student)
                    .ThenInclude(s => s!.Gender)
                .Include(sc => sc.Student)
                    .ThenInclude(s => s!.Level)
                .ToListAsync();

            // جلب المستويات مرتبة


            if (userrole == SharedLib.Fixed.Roles.Admin)
            {
                foreach (var center in Centers)
                {
                    DailyReportList.Add(await GetDailyReportByCenterIdAsync(allStdCenters, center,date));
                }
            }
            else
            {
                ///User

                //var dly=new DailyReport();
                var centerid = await CurrentCenterId();
                var Center = await _context.Centers.FirstOrDefaultAsync(x => x.Id == centerid);
                DailyReportList.Add(await GetDailyReportByCenterIdAsync(allStdCenters, Center, date));
            }

            return DailyReportList;
        }

        public Task<List<SharedLib.Entities.DailyReport>> GetDailyReportFromDateAsync(DateOnly FromDate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SharedLib.Entities.DailyReport>> GetTodayDailyReportAsync()
        {
            //string userrole = await CurUserRole();
            //List<DailyReport> DailyReportList = new List<DailyReport>();

            //var Centers = await  _context.Centers.AsNoTracking().ToListAsync();

            //// جلب جميع الطلاب النشطين مع البيانات المطلوبة
            //var allStdCenters = await _context.StdCenters
            //    .AsNoTracking()
            //    .Where(sc => sc.ToDate == null)
            //    .Include(sc => sc.Student)
            //        .ThenInclude(s => s!.Gender)
            //    .Include(sc => sc.Student)
            //        .ThenInclude(s => s!.Level)
            //    .ToListAsync();

            //// جلب المستويات مرتبة
            

            //if (userrole == SharedLib.Fixed.Roles.Admin)
            //{
            //    foreach (var center in Centers)
            //    {
            //        DailyReportList.Add(await GetDailyReportByCenterIdAsync(allStdCenters, center, DateOnly.FromDateTime(DateTime.Today)));
            //    }
            //}
            //else
            //{
            //    ///User
                
            //    //var dly=new DailyReport();
            //    var centerid = await CurrentCenterId();
            //    var Center =await _context.Centers.FirstOrDefaultAsync(x => x.Id == centerid);
            //    DailyReportList.Add(await GetDailyReportByCenterIdAsync(allStdCenters, Center));
            //}

            return await GetDailyReportForDateAsync(DateOnly.FromDateTime(DateTime.Today));
        }
        private async Task<DailyReport> GetDailyReportByCenterIdAsync(List<StdCenter> allStdCenters,  Center Center ,DateOnly ADate = default(DateOnly))
        {
            var levels = await _context.LookupValues
                .Where(lv => lv.ValueType == "Level")
                .OrderBy(lv => lv.SortOrder)
                .ToListAsync();

            var levelNames = levels.Select(l => l.Name).ToList();
            var levelMaleTotals = new Dictionary<string, int>();
            var levelFemaleTotals = new Dictionary<string, int>();
            foreach (var levelName in levelNames)
            {
                levelMaleTotals[levelName] = 0;
                levelFemaleTotals[levelName] = 0;
            }

            //TodayDate = DateOnly.FromDateTime(DateTime.Today);

            var dly = _context.DailyReports.Include(x => x.Center).FirstOrDefault(x => x.CenterId == Center.Id
                     && x.ReportDate == ADate);

            if (dly is null)
            {
                dly = new DailyReport();
                dly.ReportDate = ADate;
                var students = allStdCenters
               .Where(sc => sc.CenterId == Center.Id)
               .Select(sc => sc.Student!)
               .ToList();

                var levelMales = new Dictionary<string, int>();
                var levelFemales = new Dictionary<string, int>();
                var properties = typeof(DailyReport).GetProperties();
                int L = 0;

                foreach (var levelName in levelNames)
                {

                    levelMales[levelName] = students
                        .Count(s => s.Level?.Name == levelName && s.Gender?.Name == "ذكر");
                    levelFemales[levelName] = students
                        .Count(s => s.Level?.Name == levelName && s.Gender?.Name == "أنثى");

                    levelMaleTotals[levelName] += levelMales[levelName];
                    levelFemaleTotals[levelName] += levelFemales[levelName];
                    //////

                    properties.First(p => p.Name == $"RegMale0{L + 1}")?.SetValue(dly, levelMales[levelName]);
                    properties.First(p => p.Name == $"RegFemale0{L + 1}")?.SetValue(dly, levelFemales[levelName]);
                    L++;
                }

                dly.CenterId = Center.Id;
                dly.Center = Center;
            }

            return dly;
        }

        public async Task<GeneralResponse> UpdateDailyReportAsync(SharedLib.Entities.DailyReport dailyreport)
        {
            var existingReport = await _context.DailyReports.FindAsync(dailyreport.Id);

            if (existingReport is null)
            {
                await _context.DailyReports.AddAsync(dailyreport);
                await _context.SaveChangesAsync();
                return new GeneralResponse(true, "تم إضافة التقرير بنجاح");
            }
            else
            {
                foreach (var prop in typeof(DailyReport).GetProperties())
                {
                    var newValue = prop.GetValue(dailyreport);
                    if ((prop.Name.Contains("AttMale0")|| prop.Name.Contains("AttFemale0")) && newValue != null)
                    {
                        prop.SetValue(existingReport, newValue);
                    }
                }
                existingReport.IsLocked = dailyreport.IsLocked;
                existingReport.IsUNRWA = dailyreport.IsUNRWA;
                existingReport.Disabilities = dailyreport.Disabilities;
                existingReport.WFPBiscDist = dailyreport.WFPBiscDist;
                existingReport.WFPBiscLost = dailyreport.WFPBiscLost;

                //_context.DailyReports.Update(dailyreport);
                await _context.SaveChangesAsync();
                return new GeneralResponse(true, "تم تحديث التقرير بنجاح");
            }
        }
    }
}
