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
        private readonly AuditLogService _auditLogService;

        public EmployeeController(IStudentRepository studentRepository, IUserRepository UserRepository, IEmployeeRepository EmployeeRepository, AuditLogService auditLogService)
        {
            _employeeRepository = EmployeeRepository;
            _userRepository = UserRepository;
            _employeeRepository = EmployeeRepository;
            _auditLogService = auditLogService;
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
            await _auditLogService.LogAsync("Read", "Employee","", $"قراءة جميع الموظفين");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(long id)
        {
            var result = await _employeeRepository.GetById(id);
            if (result == null)
                return NotFound();
            await _auditLogService.LogAsync("Read", "Employee", id.ToString(), $"قراءة موظف: {result.Name}");
            return Ok(result);
        }


        [HttpGet("GetByCivilId/{CivilId}")]
        public async Task<ActionResult<EmployeeUpsertDto?>> GetByCivilId(string CivilId)
        {
            var result = await _employeeRepository.GetByCivilId(CivilId);
            if (result == null)
                return NotFound(new EmployeeUpsertDto());
            await _auditLogService.LogAsync("Read", "Employee", result.Id.ToString(), $"قراءة موظف: {result.Name}");
            return Ok(result);
        }

        [HttpGet("GetByEmpId/{EmpId}")]
        public async Task<ActionResult<EmployeeUpsertDto?>> GetByEmpId(string EmpId)
        {
            var result = await _employeeRepository.GetByEmpId(EmpId);
            if (result == null)
                return NotFound(new EmployeeUpsertDto());
            await _auditLogService.LogAsync("Read", "Employee", result.Id.ToString(), $"قراءة موظف: {result.Name}");
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(EmployeeUpsertDto employee)
        {
            var employeeToSave = (new EmployeeMapper()).ToEntity(employee);
            
            employeeToSave.EmpCenters.Add(new EmpCenter() { EmployeeId = employeeToSave.Id, CenterId = await CurrentCenterId() });
            var response = await _employeeRepository.Insert(employeeToSave);
            await _auditLogService.LogAsync("Create", "Insert Employee", employeeToSave.Id.ToString(), $"تم إضافة موظف: {employeeToSave.Name}");
            return Ok(response);
        }

        [HttpPost("AddWithCenter")]
        public async Task<ActionResult<GeneralResponse>> AddWithCenter(EmployeeWithCenter employee)
        {
            if (employee.CenterId <= 0)
            {
                employee.CenterId= await CurrentCenterId();
                //employee.Employee.EmpCenters.Add(new EmpCenter() { EmployeeId = employee.Employee.Id, CenterId = await CurrentCenterId() });
                //var response = await _employeeRepository.Insert(employee.Employee);
                //return Ok(response);
            }
            
            
            var response = await _employeeRepository.AddEmployeeWithCenter(employee.Employee, employee.CenterId);
            await _auditLogService.LogAsync("Create", "Add With Center Employee", employee.Employee.Id.ToString(), $"تم إضافة موظف: {employee.Employee.Name}");
            return Ok(response);
            
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(EmployeeUpsertDto employee)
        {
            if (employee.CenterId is null || employee.CenterId <= 0)
                employee.CenterId = await CurrentCenterId();
            var response = await _employeeRepository.UpdateWithCenter(employee);
            await _auditLogService.LogAsync("Update", "Employee", employee.Id.ToString(), $"تم تعديل موظف: {employee.Name}");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _employeeRepository.DeleteById(id);
            await _auditLogService.LogAsync("Delete", "Employee", id.ToString(), $"تم حذف موظف");
            return Ok(response);
        }

        [HttpGet("EmployeeCenterCount/{id}")]
        public async Task<ActionResult<int>> GetEmployeeCountByCenterId(long id)
        {
            var result = await _employeeRepository.GetCenterEmployeesCountAsync(id);
            await _auditLogService.LogAsync("Read", "Employee", id.ToString(), $"قراءة عدد الموظفين في المركز: {result}");
            return Ok(result);
        }


        [HttpPost("paginated")]
        public async Task<ActionResult<EmployeePaginatedResponse>> GetPaginated([FromBody] EmployeeFilterRequest request)
        {
            var employeespaginated =await _employeeRepository.GetPaginatedEmployesAsync(request , await CurrentCenterId());
            await _auditLogService.LogAsync("Read", "Employee", "", $"قراءة الموظفين بشكل مُنقَّط");
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
            
            await _auditLogService.LogAsync("Read", "Employee", "", $"تصدير الموظفين حسب التصفية: {fileName}");

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

            await _auditLogService.LogAsync("Read", "Employee", "", $"تصدير جميع الموظفين: {fileName}");

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpGet("Managers")]
        public async Task<ActionResult<List<Employee>>> GetAllManagers()
        {
            var managers = await _employeeRepository.GetAllManagers();
            return Ok(managers);
        }
        [HttpGet("CenterManager")]
        public async Task<ActionResult<Employee>> GetCenterManagers(long centerId)
        {
            var managers = await _employeeRepository.GetCenterManagers(centerId);
            return Ok(managers);
        }

        [HttpPost("IsCivilIdDuplicate")]
        public async Task<ActionResult<Employee?>> IsCivilIdDuplicateAsync(EmployeeDuplicateCheckRequest request)
        {
            var isDuplicate = await _employeeRepository.IsCivilIdDuplicateAsync(request);
            if (isDuplicate == null)
                return Ok(new Employee() { Id=-1});
            else
                return Ok(isDuplicate);
        }

        [HttpPost("IsEmpIdDuplicate")]
        public async Task<ActionResult<Employee?>> IsEmpIdDuplicateAsync(EmployeeDuplicateCheckRequest request)
        {
            var isDuplicate = await _employeeRepository.IsEmpIdDuplicateAsync(request);
            if (isDuplicate == null)
                return Ok(new Employee() { Id = -1 });
            else
                return Ok(isDuplicate);
        }
        [HttpPost("IsEmployeeDuplicate")]
        public async Task<ActionResult<Employee?>> IsEmployeeDuplicateAsync(EmployeeDuplicateCheckRequest request)
        {
            var isDuplicate = await _employeeRepository.IsEmployeeDuplicateAsync(request);
            if (isDuplicate == null)
                return Ok(new Employee() { Id = -1 });
            else
                return Ok(isDuplicate);
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