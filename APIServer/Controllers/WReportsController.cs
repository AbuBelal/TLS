using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Entities;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WReportsController : ControllerBase
    {
        private readonly IWReportRepository _repository;

        public WReportsController(IWReportRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WReport>>> GetReports()
        {
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
        public async Task<ActionResult<WReport>> CreateReport([FromBody] WReport report)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdReport = await _repository.AddAsync(report);

            // إرجاع 201 Created مع رابط الـ Resource الجديد
            return CreatedAtAction(nameof(GetReport), new { id = createdReport.Id }, createdReport);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateReport(long id, [FromBody] WReport report)
        {
            if (id != report.Id)
                return BadRequest(new { message = "ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingReport = await _repository.GetByIdAsync(id);
            if (existingReport == null)
                return NotFound(new { message = $"Report with ID {id} not found." });

            // ملاحظة: في بيئة العمل الحقيقية، يفضل استخدام AutoMapper هنا لتحديث الحقول
            existingReport.WReportBegin = report.WReportBegin;
            existingReport.WReportEnd = report.WReportEnd;
            existingReport.WReportNo = report.WReportNo;
            existingReport.Comments = report.Comments;

            await _repository.UpdateAsync(existingReport);

            return NoContent(); // 204 No Content
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteReport(long id)
        {
            var existingReport = await _repository.GetByIdAsync(id);
            if (existingReport == null)
                return NotFound(new { message = $"Report with ID {id} not found." });

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
