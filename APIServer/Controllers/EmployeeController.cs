using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;

        public EmployeeController(IStudentRepository studentRepository, IUserRepository UserRepository, IEmployeeRepository EmployeeRepository)
        {
            _employeeRepository = EmployeeRepository;
            _userRepository = UserRepository;
            _employeeRepository = EmployeeRepository;
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




        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Employee>>> GetAll()
        {
            var result = await _employeeRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(long id)
        {
            var result = await _employeeRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


        [HttpGet("GetByCivilId/{CivilId}")]
        public async Task<ActionResult<EmployeeUpsertDto?>> GetByCivilId(string CivilId)
        {
            var result = await _employeeRepository.GetByCivilId(CivilId);
            if (result == null)
                return NotFound(null);
            return Ok(result);
        }

        [HttpGet("GetByEmpId/{EmpId}")]
        public async Task<ActionResult<EmployeeUpsertDto?>> GetByEmpId(string EmpId)
        {
            var result = await _employeeRepository.GetByEmpId(EmpId);
            if (result == null)
                return NotFound(null);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(EmployeeUpsertDto employee)
        {
            var employeeToSave = (new EmployeeMapper()).ToEntity(employee);
            
            employeeToSave.EmpCenters.Add(new EmpCenter() { EmployeeId = employeeToSave.Id, CenterId = await CurrentCenterId() });
            var response = await _employeeRepository.Insert(employeeToSave);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(EmployeeUpsertDto employee)
        {
            var employeeToSave = (new EmployeeMapper()).ToEntity(employee);
            
            var response = await _employeeRepository.Update(employeeToSave);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _employeeRepository.DeleteById(id);
            return Ok(response);
        }

        [HttpGet("EmployeeCenterCount/{id}")]
        public async Task<ActionResult<int>> GetEmployeeCountByCenterId(long id)
        {
            var result = await _employeeRepository.GetCenterEmployeesCountAsync(id);
            return Ok(result);
        }


        [HttpPost("paginated")]
        public async Task<ActionResult<EmployeePaginatedResponse>> GetPaginated([FromBody] EmployeeFilterRequest request)
        {
            var employeespaginated =await _employeeRepository.GetPaginatedEmployesAsync(request , await CurrentCenterId());
            return Ok(employeespaginated);
        }
    }
}