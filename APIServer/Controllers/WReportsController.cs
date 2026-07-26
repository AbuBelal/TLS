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
    public class WReportsController : ControllerBase
    {
        private readonly IWReportRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AuditLogService _auditLogService;

        public WReportsController(IWReportRepository repository, IUserRepository UserRepository, IEmployeeRepository EmployeeRepository, AuditLogService auditLogService)
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
        public async Task<ActionResult<IEnumerable<WReport>>> GetReports()
        {
            var CurCenterId = await CurrentCenterId();

            if(CurCenterId >0)
            {
                var Centerreports = await _repository.GetReportsByCenterIdAsync(CurCenterId);
                return Ok(Centerreports);
            }

            var reports = await _repository.GetAllAsync();
            return Ok(reports);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<WReport>> GetReport(long id)
        {
            var report = await _repository.GetByIdAsync(id);
            if (report == null)
                return NotFound(new { message = $"Report with ID {id} not found." });

            return Ok(report);
        }

        [HttpPost]
        public async Task<GeneralResponse> CreateReport([FromBody] WReport report)
        {
            if (!ModelState.IsValid)
                return new GeneralResponse(false, "البيانات غير مكتملة", 0);

            var createdReport = await _repository.AddAsync(report);

            return new GeneralResponse(true, "تم إنشاء الأسبوع للتقارير الأسبوعية", createdReport.Id);
            // إرجاع 201 Created مع رابط الـ Resource الجديد
            //return CreatedAtAction(nameof(GetReport), new { id = createdReport.Id }, createdReport);
        }

        [HttpPut]
        public async Task<GeneralResponse> UpdateReport([FromBody] WReport report)
        {
          
            if (!ModelState.IsValid)
                return new GeneralResponse(false, "البيانات غير مكتملة", 0);

            var existingReport = await _repository.GetByIdAsync(report.Id);
            if (existingReport == null)
                return new GeneralResponse(false, "التقرير غير موجود", 0);

            // ملاحظة: في بيئة العمل الحقيقية، يفضل استخدام AutoMapper هنا لتحديث الحقول
            existingReport.WReportBegin = report.WReportBegin;
            existingReport.WReportEnd = report.WReportEnd;
            existingReport.WReportNo = report.WReportNo;
            existingReport.Comments = report.Comments;

            await _repository.UpdateAsync(existingReport);

            return new GeneralResponse(true, "تم تحديث مجموعة التقارير الأسبوعية", existingReport.Id);
        }

        [HttpDelete("{id:long}")]
        public async Task<GeneralResponse> DeleteReport(long id)
        {
            var existingReport = await _repository.GetByIdAsync(id);
            if (existingReport == null)
                return new GeneralResponse(false, "التقرير غير موجود");

            await _repository.DeleteAsync(id);

            return new GeneralResponse(true,"تم حذف مجموعة التقارير الأسبوعية");
        }
    }
}
