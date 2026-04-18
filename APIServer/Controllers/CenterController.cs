using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CenterController : ControllerBase
    {
        private readonly ICenterRepository _centerRepository;
        private readonly AuditLogService _auditLogService;
        public CenterController(ICenterRepository centerRepository, AuditLogService auditLogService)
        {
            _centerRepository = centerRepository;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Center>>> GetAll()
        {
            var result = await _centerRepository.GetAll();
            await _auditLogService.LogAsync("Read", "Center", "", $"قراءة جميع المراكز");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Center>> GetById(long id)
        {
            var result = await _centerRepository.GetById(id);
            if (result == null)
                return NotFound();
            await _auditLogService.LogAsync("Read", "Center", id.ToString(), $"قراءة مركز: {result.Name}");
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(Center center)
        {
            var response = await _centerRepository.Insert(center);
            await _auditLogService.LogAsync("Create", "Center", center.Id.ToString(), $"تم إضافة مركز: {center.Name}");
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(CenterUpsertDto center)
        {
            var response = await _centerRepository.Update(center);
            await _auditLogService.LogAsync("Update", "Center", center.Id.ToString(), $"تم تعديل مركز: {center.Name}");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _centerRepository.DeleteById(id);
            await _auditLogService.LogAsync("Delete", "Center", id.ToString(), $"تم حذف مركز");
            return Ok(response);
        }

        [HttpGet("my-center")]
        [Authorize(Roles = $"{SharedLib.Fixed.Roles.User},{SharedLib.Fixed.Roles.User}")]
        public async Task<ActionResult<Center>> GetMyCenter()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var center = await _centerRepository.GetByUserIdAsync(userId);
            await _auditLogService.LogAsync("Read", "Center", "", $"قراءة مركز المستخدم: {center?.Name}");
            return center is null
                ? NotFound(new GeneralResponse(false, "لا يوجد مركز مرتبط بحسابك.", 0))
                : Ok(center);
        }

        /// <summary>
        /// PUT /api/Center/my-center
        /// يعدّل بيانات المركز — لمدير المركز فقط، مع التحقق من الملكية
        /// </summary>
        [HttpPut("my-center")]
        [Authorize(Roles = SharedLib.Fixed.Roles.User)]
        public async Task<ActionResult<GeneralResponse>> UpdateMyCenter([FromBody] CenterUpsertDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _centerRepository.UpdateByUserAsync(dto, userId);
            await _auditLogService.LogAsync("Update", "Center", dto.Id.ToString(), $"تم تعديل مركز المستخدم: {dto.Name}");
            return Ok(result);
        }
    }
}