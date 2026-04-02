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
[Authorize(Roles = Roles.Admin)]   // فقط المدير يصل لهذا الـ endpoint
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
}
