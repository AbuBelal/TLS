using APIServerLib.Repositories.Interfaces;
using Azure;
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
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public StudentController(IStudentRepository studentRepository, IUserRepository UserRepository , IEmployeeRepository EmployeeRepository)
        {
            _studentRepository = studentRepository;
            _userRepository = UserRepository;
            _employeeRepository = EmployeeRepository;
        }
        #region CurUser CurEmp Details
        private async Task<ApplicationUser> CurrentUser ()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _userRepository.GetById(userId);
        }
        private async Task<Employee> CurrentEmployee ()
        {
            var CurUser = await CurrentUser();
            return await _employeeRepository.GetById(CurUser.EmployeeId ?? 0);
        }
        private async Task<long> CurrentCenterId ()
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
        public async Task<ActionResult<List<Student>>> GetAll()
        {
            var result = await _studentRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetById(long id)
        {
            var result = await _studentRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(Student student)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var FullUser = await _userRepository.GetById(userId);
            //var Employee = await _employeeRepository.GetById(FullUser.EmployeeId ?? 0);
            //var Employee = await CurrentEmployee();
            var CurCenter = await CurrentCenterId();
            if (CurCenter >0)
            { 
                //var centerid= Employee.EmpCenters.OrderByDescending(ec => ec.FromDate).FirstOrDefault()?.CenterId;
                var response = await _studentRepository.AddStudentWithCenter(student , CurCenter);

              return Ok(response);
            }
            return new GeneralResponse(false, "لا يمكن إضافة طالب ، تأكد من البيانات أو صلاحيات المستخدم", 0);
        }

        [HttpPost("AddWithCenter")]
        public async Task<ActionResult<GeneralResponse>> AddWithCenter(StdWithCenterId student)
        {
            if (student.CenterId <= 0)
            {
                return await Insert(student.Student);
            }
            else
            {
                var response = await _studentRepository.AddStudentWithCenter(student.Student, student.CenterId);
                return Ok(response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(Student student)
        {
            var response = await _studentRepository.Update(student);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _studentRepository.DeleteById(id);
            return Ok(response);
        }

        [HttpGet("StudentCenterCount/{id}")]
        public async Task<ActionResult<int>> GetStudentCountByCenterId(long id)
        {
            var result = await _studentRepository.GetCenterStudentsCountAsync(id);
            return Ok(result);
        }



        // ========================================
        //  GET: api/Student/paginated
        //  Server-Side Filtering + Pagination
        // ========================================
        [HttpGet("paginated")]
        public async Task<ActionResult<PaginatedResponse<StudentDto>>>
            GetPaginated([FromQuery] StudentFilterRequest request)
        {
            var CurCenter = await CurrentCenterId() ;
            var response = await _studentRepository.GetPaginatedStudentsAsync(request, CurCenter);
            return Ok(response);
        }


        /// <summary>
        /// GET /api/Student/export/filtered
        /// تصدير الطلاب المعروضين حسب التصفية الحالية
        /// </summary>
        [HttpGet("export/filtered")]
        public async Task<IActionResult> ExportFiltered([FromQuery] StudentFilterRequest request)
        {
            var centerId = await CurrentCenterId();
            //if (centerId == 0) return BadRequest("لا يوجد مركز مرتبط بحسابك.");

            // جلب اسم المركز لعنوان الملف
            var centerName = await GetCenterNameAsync(centerId);

            // جلب الطلاب بنفس فلاتر الصفحة لكن بدون pagination
            var students = await _studentRepository.GetFilteredForExportAsync(request, centerId);

            // توصيف عنوان الورقة حسب الفلاتر المطبقة
            var sheetTitle = BuildSheetTitle(request);

            var bytes = StudentExportService.GenerateExcel(students, sheetTitle, centerName);
            var fileName = $"طلاب_{centerName}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        /// <summary>
        /// GET /api/Student/export/all
        /// تصدير جميع طلاب المركز بدون فلاتر
        /// </summary>
        [HttpGet("export/all")]
        public async Task<IActionResult> ExportAll()
        {
            var centerId = await CurrentCenterId();
            //if (centerId == 0) return BadRequest("لا يوجد مركز مرتبط بحسابك.");

            var centerName = await GetCenterNameAsync(centerId);
            var students = await _studentRepository.GetAllByCenterAsync(centerId);

            var bytes = StudentExportService.GenerateExcel(
                students, "جميع الطلاب", centerName);

            var fileName = $"جميع_طلاب_{centerName}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // ── دوال مساعدة خاصة ───────────────────────────────────────────

        private async Task<string> GetCenterNameAsync(long centerId)
        {
            if(centerId == 0)   return "بدون_مركز";
            // نستخدم الـ DbContext مباشرةً عبر ICenterRepository
            // أو عبر StudentRepository — الأبسط هنا استخدام UserRepository
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

        private static string BuildSheetTitle(StudentFilterRequest req)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(req.SearchText))
                parts.Add($"بحث: {req.SearchText}");
            if (!string.IsNullOrWhiteSpace(req.Gender))
                parts.Add($"الجنس: {req.Gender}");
            if (!string.IsNullOrWhiteSpace(req.Level))
                parts.Add($"المستوى: {req.Level}");

            return parts.Count > 0
                ? "تصفية: " + string.Join(" | ", parts)
                : "المعروض على الشاشة";
        }

    }
}