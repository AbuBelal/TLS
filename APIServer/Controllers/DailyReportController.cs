using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportController : ControllerBase
    {
        private readonly IDailyReportRepository _repo;
        private readonly AuditLogService _auditLogService;

        public DailyReportController(IDailyReportRepository repo, AuditLogService auditLogService)
        {
            _repo = repo;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyReport()
        {
            var report = await _repo.GetTodayDailyReportAsync();
            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDailyReport([FromBody] DailyReport dailyReport)
        {
            var result = await _repo.UpdateDailyReportAsync(dailyReport);
            return Ok(result);
        }


        [HttpGet("for-date")]
        public async Task<IActionResult> GetDailyReportForDate([FromQuery] DateOnly date)
        {
            var report = await _repo.GetDailyReportForDateAsync(date);
            return Ok(report);
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export(DateOnly date)
        {
            var report = await _repo.GetDailyReportForDateAsync(date);
            var bytes = DailyReportExportService.GenerateExcel(report, " التقرير اليومي");
            var fileName = $"جميع_التقارير_{date.ToString("yyyy-MM-dd")}.xlsx";

            await _auditLogService.LogAsync("Read", "Export Daily Report", "", $"تصدير التقرير اليومي: {fileName}");

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpGet("building-total-dist")]
        public async Task<ActionResult<decimal>> GetBuildingTotalDist([FromQuery] string? BuildingId = null)
        {
            var total = await _repo.GetBuildingTotalDistAsync(BuildingId);
            return Ok(total);
        }
    }
}
