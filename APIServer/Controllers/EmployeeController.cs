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

        [HttpPost("AddWithCenter")]
        public async Task<ActionResult<GeneralResponse>> AddWithCenter(EmployeeWithCenter employee)
        {
            if (employee.CenterId == 0)
            {
                employee.Employee.EmpCenters.Add(new EmpCenter() { EmployeeId = employee.Employee.Id, CenterId = await CurrentCenterId() });
                var response = await _employeeRepository.Insert(employee.Employee);
                return Ok(response);
            }
            else
            {
                var response = await _employeeRepository.AddEmployeeWithCenter(employee.Employee, employee.CenterId);
                return Ok(response);
            }
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


        /// <summary>
        /// POST /api/Employee/export/filtered
        /// تصدير الموظفين المعروضين حسب التصفية الحالية
        /// ملاحظة: نستخدم POST لأن paginated يستخدم POST (اتساق مع الـ API)
        /// </summary>
        [HttpPost("export/filtered")]
        public async Task<IActionResult> ExportFiltered([FromBody] EmployeeFilterRequest request)
        {

            var centerId = await CurrentCenterId();
            //if (centerId == 0) return BadRequest("لا يوجد مركز مرتبط بحسابك.");

            var centerName = await GetCenterNameAsync(centerId);
            var employees = await _employeeRepository.GetFilteredForExportAsync(request, centerId);
            var sheetTitle = BuildSheetTitle(request);

            var bytes = EmployeeExportService.GenerateExcel(employees, sheetTitle, centerName);
            var fileName = $"موظفون_{centerName}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        /// <summary>
        /// GET /api/Employee/export/all
        /// تصدير جميع موظفي المركز بدون فلاتر
        /// </summary>
        [HttpGet("export/all")]
        public async Task<IActionResult> ExportAll()
        {
            var centerId = await CurrentCenterId();
            //if (centerId == 0) return BadRequest("لا يوجد مركز مرتبط بحسابك.");

            var centerName = await GetCenterNameAsync(centerId);
            var employees = await _employeeRepository.GetAllByCenterAsync(centerId);

            var bytes = EmployeeExportService.GenerateExcel(employees, "جميع الموظفين", centerName);
            var fileName = $"جميع_موظفي_{centerName}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // ── دالة مساعدة ────────────────────────────────────────────────
        private async Task<string> GetCenterNameAsync(long centerId)
        {
            //try
            //{
            //    var user = await CurrentUser();
            //    return user?.Employee?.EmpCenters
            //        .OrderByDescending(x => x.FromDate)
            //        .FirstOrDefault()?.Center?.Name ?? "المركز";
            //}
            //catch { return "المركز"; }


            if (centerId == 0) return "بدون_مركز";
            try
            {
                var user = await CurrentUser();
                var emp = user?.Employee;
                var ec = emp?.EmpCenters
                    .OrderByDescending(x => x.FromDate)
                    .FirstOrDefault();
                return ec?.Center?.Name ?? centerId.ToString();
            }
            catch { return centerId.ToString(); }

        }

        private static string BuildSheetTitle(EmployeeFilterRequest req)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(req.SearchText)) parts.Add($"بحث: {req.SearchText}");
            if (!string.IsNullOrWhiteSpace(req.Gender)) parts.Add($"الجنس: {req.Gender}");
            if (!string.IsNullOrWhiteSpace(req.Job)) parts.Add($"الوظيفة: {req.Job}");

            return parts.Count > 0
                ? "تصفية: " + string.Join(" | ", parts)
                : "المعروض على الشاشة";
        }

    }
}