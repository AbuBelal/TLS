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

        public CenterController(ICenterRepository centerRepository)
        {
            _centerRepository = centerRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Center>>> GetAll()
        {
            var result = await _centerRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Center>> GetById(long id)
        {
            var result = await _centerRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(Center center)
        {
            var response = await _centerRepository.Insert(center);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(CenterUpsertDto center)
        {
            var response = await _centerRepository.Update(center);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _centerRepository.DeleteById(id);
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
            return Ok(result);
        }
    }
}