using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportController : ControllerBase
    {
        private readonly IDailyReportRepository _repo;

        public DailyReportController(IDailyReportRepository repo)
            => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetDailyReport()
        {
            var report = await _repo.GetTodayDailyReportAsync();
            return Ok(report);
        }
    }
}
