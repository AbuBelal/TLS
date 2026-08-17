using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttRecordController : ControllerBase
    {
        private readonly IAttRecordRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AuditLogService _auditLogService;

        public AttRecordController(IAttRecordRepository repository, IUserRepository UserRepository, IEmployeeRepository EmployeeRepository, AuditLogService auditLogService)
        {
            _repository = repository;
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
            return await _employeeRepository.GetById(CurUser?.EmployeeId ?? 0);
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

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateMonthlyAttendance([FromBody] GenerateAttendanceRequest request)
        {
            // تحقق مبدئي من صحة التواريخ
            if (request.Year < 2000 || request.Month < 1 || request.Month > 12)
            {
                return BadRequest("بيانات السنة أو الشهر غير صالحة.");
            }

            try
            {
                // استدعاء دالة التوليد من الـ Repository
                int generatedCount = await _repository.GenerateMonthlyAttendanceAsync(
                    request.Year,
                    request.Month,
                    request.HolidayCode
                );

                var response = new GeneralResponse
                (true,
                    $"تم إنشاء {generatedCount} سجل دوام بنجاح.", 0
                //Count = generatedCount
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                // يفضل تسجيل الخطأ هنا باستخدام ILogger
                return StatusCode(500, $"حدث خطأ داخلي أثناء توليد اللوائح: {ex.Message}");
            }
        }


        [HttpGet("center/{year}/{month}")]
        public async Task<IActionResult> GetCenterAttendance(int year, int month)
        {
            var centerId = await CurrentCenterId();
            var records = await _repository.GetAttendanceByCenterAsync(centerId, year, month);

            if (records == null || !records.Any())
                return NotFound("لا توجد سجلات دوام لهذا المركز في الشهر المحدد.");

            // لتجنب مشكلة الـ Circular Reference عند إرسال البيانات كـ JSON
            // يمكنك استخدام DTO، أو استخدام خيارات JSON لتجاهل الـ Reference Cycles
            return Ok(records);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAttendance([FromBody] List<AttendanceRecord> records)
        {
            if (records == null || !records.Any())
                return BadRequest("لم يتم إرسال أي بيانات للتحديث.");

            try
            {
                var result = await _repository.UpdateAttendanceRecordsAsync(records);
                return Ok(new { Message = "تم حفظ التعديلات بنجاح." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء الحفظ: {ex.Message}");
            }
        }


    }
}
