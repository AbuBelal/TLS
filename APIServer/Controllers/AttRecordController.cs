using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpPost()]
        public async Task<GeneralResponse> GenerateMonthlyAttendance([FromBody] GenerateAttendanceRequest request)
        {
            // تحقق مبدئي من صحة التواريخ
            if (request.Year < 2000 || request.Month < 1 || request.Month > 12)
            {
                return new GeneralResponse(false, "بيانات السنة أو الشهر غير صالحة.");
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

                return response;
            }
            catch (Exception ex)
            {
                // يفضل تسجيل الخطأ هنا باستخدام ILogger
                return new GeneralResponse(false, $"حدث خطأ داخلي أثناء توليد اللوائح: {ex.Message}");
            }
        }


        [HttpGet("{year}/{month}")]
        public async Task<List<AttendanceRecord>> GetCenterAttendance(int year, int month)
        {
            var centerId = await CurrentCenterId();
            List<AttendanceRecord> records = await _repository.GetAttendanceByCenterAsync(centerId, year, month);

            if (records == null || !records.Any())
                return new List<AttendanceRecord>();// "لا توجد سجلات دوام لهذا المركز في الشهر المحدد.");

            // لتجنب مشكلة الـ Circular Reference عند إرسال البيانات كـ JSON
            // يمكنك استخدام DTO، أو استخدام خيارات JSON لتجاهل الـ Reference Cycles
            return records;
        }

        [HttpPut()]
        public async Task<GeneralResponse> UpdateAttendance([FromBody] List<AttendanceRecord> records)
        {
            if (records == null || !records.Any())
                return new GeneralResponse(true, "لم يتم إرسال أي بيانات للتحديث.", 0);

            try
            {
                var result = await _repository.UpdateAttendanceRecordsAsync(records);
                return new GeneralResponse(true, "تم حفظ التعديلات بنجاح.", 0);
            }
            catch (Exception ex)
            {
                return new GeneralResponse(true, $"حدث خطأ أثناء الحفظ: {ex.Message}", 0);
            }
        }

        [HttpPost("lock")]
        public async Task<GeneralResponse> LockAttendance(GenerateAttendanceRequest request)
        {
            try
            {
                await _repository.LockAttendanceRecordsAsync(request.Year, request.Month, request.Lock??true);
                return new GeneralResponse(true, "تم قفل سجلات الدوام لهذا الشهر بنجاح.", 0);
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, $"حدث خطأ أثناء القفل: {ex.Message}", 0);
            }

        }
    }
}
