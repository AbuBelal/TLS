using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Entities;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        public async Task<ActionResult<CenterDashboardDto>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _dashboardRepository.GetCenterDashboardAsync(userId);
            return Ok(result);
        }
    }
}
