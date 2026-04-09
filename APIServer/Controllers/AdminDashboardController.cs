// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServer/Controllers/AdminDashboardController.cs           ║
// ╚══════════════════════════════════════════════════════════════╝

using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Fixed;

namespace APIServer.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = Roles.Admin)]   // فقط المدير يصل لهذا الـ endpoint
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardRepository _repo;

    public AdminDashboardController(IAdminDashboardRepository repo)
        => _repo = repo;

    /// <summary>
    /// GET /api/AdminDashboard
    /// يُعيد إحصائيات جميع المراكز للمدير العام
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<AdminDashboardDto>> Get()
    {
        var result = await _repo.GetAdminDashboardAsync();
        return Ok(result);
    }

    /// <summary>
    /// GET /api/AdminDashboard/detailed-report
    /// تقرير مفصل لجميع المراكز
    /// </summary>
    [AllowAnonymous]
    [HttpGet("detailed-report")]
    public async Task<ActionResult<DetailedCentersReportDto>> GetDetailedReport()
    {
        var result = await _repo.GetDetailedCentersReportAsync();
        return Ok(result);
    }

    /// <summary>
    /// GET /api/AdminDashboard/export/detailed-report
    /// تصدير التقرير المفصل إلى Excel
    /// </summary>

    [HttpGet("export/detailed-report")]
    public async Task<IActionResult> ExportDetailedReport()
    {
        var report = await _repo.GetDetailedCentersReportAsync();

        var bytes = DetailedCentersReportExportService.GenerateExcel(report);
        var fileName = $"تقرير_المراكز_المفصل_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
