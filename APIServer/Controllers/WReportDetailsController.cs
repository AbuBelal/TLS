using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WReportDetailsController : ControllerBase
    {
        private readonly IWReportDetailRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AuditLogService _auditLogService;
        public WReportDetailsController(IWReportDetailRepository repository, IUserRepository UserRepository, IEmployeeRepository EmployeeRepository, AuditLogService auditLogService)
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
        public async Task<ActionResult<IEnumerable<WReportDetail>>> GetReportDetails()
        {
            var details = await _repository.GetAllAsync();
            return Ok(details);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<WReportDetail>> GetReportDetail(long id)
        {
            var detail = await _repository.GetByIdAsync(id);
            if (detail == null)
                return NotFound(new { message = $"Report Detail with ID {id} not found." });

            return Ok(detail);
        }

        // Endpoint إضافي للبحث عن تفاصيل تقرير معين
        [HttpGet("by-report/{wReportId:long}")]
        public async Task<ActionResult<IEnumerable<WReportDetail>>> GetDetailsByReportId(long wReportId)
        {
            var details = await _repository.GetByWReportIdAsync(wReportId);
            return Ok(details);
        }

        [HttpPost]
        public async Task<GeneralResponse> CreateReportDetail([FromBody] WReportDetail reportDetail)
        {
            if (reportDetail.CenterId is null || reportDetail.CenterId == 0)
                reportDetail.CenterId = await CurrentCenterId();

            if (!ModelState.IsValid)
                return new GeneralResponse(false, "البيانات غير مكتملة", 0);

            var createdDetail = await _repository.AddAsync(reportDetail);

            return new GeneralResponse(true, "تم إنشاء تفاصيل التقرير بنجاح", createdDetail.Id);
        }

        [HttpPut()]
        public async Task<GeneralResponse> UpdateReportDetail([FromBody] WReportDetail reportDetail)
        {
           

            if (!ModelState.IsValid)
                return new GeneralResponse(false, "البيانات غير مكتملة", 0);    

            // التحقق من وجود العنصر أولاً
            var existingDetail = await _repository.GetByIdAsync(reportDetail.Id);
            if (existingDetail == null)
                return new GeneralResponse(false, $"Report Detail with ID {reportDetail.Id} not found.", 0);

            // يمكنك إما تحديث الحقول يدوياً أو استخدام Update للملف بالكامل
            // لأن الكلاس يحتوي على حقول كثيرة جداً، الطريقة الأفضل في هذه المرحلة بدون DTO:
            _repository.UpdateAsync(reportDetail); // يفترض أن الكائن القادم يحتوي على كل البيانات المطلوبة

            return new GeneralResponse(true, "تم تحديث تفاصيل التقرير بنجاح", reportDetail.Id);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteReportDetail(long id)
        {
            var existingDetail = await _repository.GetByIdAsync(id);
            if (existingDetail == null)
                return NotFound(new { message = $"Report Detail with ID {id} not found." });

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
