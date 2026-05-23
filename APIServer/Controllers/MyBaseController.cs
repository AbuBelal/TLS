using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Entities;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MyBaseController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        public MyBaseController(IEmployeeRepository employeeRepository, IUserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

  
        public async Task<ApplicationUser> CurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;
            return await _userRepository.GetById(userId);
        }
        public async Task<Employee> CurrentEmployee()
        {
            var CurUser = await CurrentUser();
            if (CurUser is null) return null; // حماية من الـ Null
            return await _employeeRepository.GetById(CurUser.EmployeeId ?? 0);
        }
        public async Task<long> CurrentCenterId()
        {
            var Employee = await CurrentEmployee();
            return
                Employee is null ? 0 :
                Employee.EmpCenters
                .OrderByDescending(ec => ec.FromDate)
                .FirstOrDefault()?
                .CenterId ?? 0;
        }
        
    }
}
