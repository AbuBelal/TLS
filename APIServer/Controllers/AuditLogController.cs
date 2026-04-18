using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Fixed;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuditLogDto>>> GetAll()
        {
            var result = await _auditLogRepository.GetAll();
            return Ok(result);
        }

        [HttpPost("paginated")]
        public async Task<ActionResult<PaginatedResponse<AuditLogDto>>> GetPaginated([FromBody] AuditLogFilterRequest request)
        {
            var result = await _auditLogRepository.GetPaginated(request);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<AuditLogDto>>> GetByUserId(string userId)
        {
            var result = await _auditLogRepository.GetByUserId(userId);
            return Ok(result);
        }

        [HttpGet("entity/{entityType}")]
        public async Task<ActionResult<List<AuditLogDto>>> GetByEntityType(string entityType)
        {
            var result = await _auditLogRepository.GetByEntityType(entityType);
            return Ok(result);
        }
    }
}
