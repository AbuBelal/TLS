using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        public AttendanceController(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUserRepository userRepository)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }


        #region CurUser CurEmp Details
        private async Task<ApplicationUser> CurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        #endregion

        [HttpPost("get-attendancesAvg")]
        public async Task<ActionResult<List<DailyAttendance>>> GetAttendancesAvg(AttendanceRequest request)
        {
            if(request.CenterId == 0)
            {
                request.CenterId = await CurrentCenterId();
            }
            var attendances = await _attendanceRepository.GetAttendancesAsync(request.CenterId, request.From, request.To);
            return Ok(attendances);
        }
    }
}
