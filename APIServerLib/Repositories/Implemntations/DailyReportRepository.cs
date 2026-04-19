using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
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

        public Task<List<SharedLib.Entities.DailyReport>> GetDailyReportForDateAsync(DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<List<SharedLib.Entities.DailyReport>> GetDailyReportFromDateAsync(DateOnly FromDate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SharedLib.Entities.DailyReport>> GetTodayDailyReportAsync()
        {
            string userrole = await CurUserRole();
            List<DailyReport> DailyReportList = new List<DailyReport>();
            var Centers =  _context.Centers.ToList();
            var TodayDate = DateOnly.FromDateTime(DateTime.Today);

            if (userrole == SharedLib.Fixed.Roles.Admin)
            {
                var stdCount = _context.Students.GroupBy(x =>new { x.StdCenters.OrderByDescending(x => x.FromDate).First().CenterId , x.LevelId })
                    .Select(   g => g.Count()).ToList();
                foreach (var c in Centers)
                {
                    var dly = _context.DailyReports.FirstOrDefault(x=>x.CenterId == c.Id
                    && x.ReportDate==TodayDate);

                    if(dly is null)
                    {
                        dly = new DailyReport();
                        dly.CenterId = c.Id;
                        dly.Center = c;
                        
                        dly.RegMale01 = 0;
                    }
                    DailyReportList.Add(dly);
                }
            }
            else
            {
                var dly=new DailyReport();
                dly.CenterId =await CurrentCenterId();
            }

            return DailyReportList;
        }

        public Task<GeneralResponse> UpdateDailyReportAsync(SharedLib.Entities.DailyReport dailyreport)
        {
            throw new NotImplementedException();
        }
    }
}
