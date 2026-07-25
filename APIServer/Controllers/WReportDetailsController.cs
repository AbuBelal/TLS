using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Entities;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WReportDetailsController : ControllerBase
    {
        private readonly IWReportDetailRepository _repository;

        public WReportDetailsController(IWReportDetailRepository repository)
        {
            _repository = repository;
        }

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
        public async Task<ActionResult<WReportDetail>> CreateReportDetail([FromBody] WReportDetail reportDetail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdDetail = await _repository.AddAsync(reportDetail);

            return CreatedAtAction(nameof(GetReportDetail), new { id = createdDetail.Id }, createdDetail);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateReportDetail(long id, [FromBody] WReportDetail reportDetail)
        {
            if (id != reportDetail.Id)
                return BadRequest(new { message = "ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // التحقق من وجود العنصر أولاً
            var existingDetail = await _repository.GetByIdAsync(id);
            if (existingDetail == null)
                return NotFound(new { message = $"Report Detail with ID {id} not found." });

            // يمكنك إما تحديث الحقول يدوياً أو استخدام Update للملف بالكامل
            // لأن الكلاس يحتوي على حقول كثيرة جداً، الطريقة الأفضل في هذه المرحلة بدون DTO:
            _repository.UpdateAsync(reportDetail); // يفترض أن الكائن القادم يحتوي على كل البيانات المطلوبة

            return NoContent(); // 204 No Content
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
