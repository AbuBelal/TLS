using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServer.Controllers
{
    // Controllers/InComeController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class InComeController : ControllerBase
    {
        private readonly IInComeRepository _repository;

        public InComeController(IInComeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InComeDto>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dtos = items.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<InComeDto>> GetById(long id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound(new { message = "الإيراد غير موجود" });

            return Ok(MapToDto(item));
        }

        [HttpGet("by-center/{centerId:long}")]
        public async Task<ActionResult<IEnumerable<InComeDto>>> GetByCenter(long centerId)
        {
            var items = await _repository.GetByCenterIdAsync(centerId);
            return Ok(items.Select(MapToDto));
        }

        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<InComeDto>>> GetByDateRange(
            [FromQuery] DateOnly from,
            [FromQuery] DateOnly to)
        {
            if (from > to)
                return BadRequest(new { message = "تاريخ البداية يجب أن يكون قبل تاريخ النهاية" });

            var items = await _repository.GetByDateRangeAsync(from, to);
            return Ok(items.Select(MapToDto));
        }

        [HttpGet("total")]
        public async Task<ActionResult<object>> GetTotal([FromQuery] long? centerId)
        {
            var total = await _repository.GetTotalAmountAsync(centerId);
            return Ok(new { totalAmount = total });
        }

        [HttpGet("Buildingtotal")]
        public async Task<ActionResult<decimal>> GetBuildingTotal([FromQuery] string? BuildingId)
        {
            var total = await _repository.GetBuildingTotalAmountAsync(BuildingId);
            return Ok(total);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteByCenter([FromQuery] long Id)
        {
            var deletedCount = await _repository.DeleteAsync(Id);
            return Ok(new { message = $"{deletedCount} إيراد تم حذفها" });
        }

        [HttpPost]
        public async Task<ActionResult<InComeDto>> Create([FromBody] CreateInComeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var income = new InCome
            {
                Name = dto.Name,
                Comments = dto.Comments,
                Date = dto.Date,
                Qnty = dto.Qnty,
                CenterId = dto.CenterId,
                RecipientName = dto.RecipientName
            };

            var created = await _repository.CreateAsync(income);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                MapToDto(created));
        }

        [HttpPut()]
        public async Task<ActionResult<InComeDto>> Update([FromBody] UpdateInComeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _repository.UpdateAsync(dto.Id, new InCome
            {
                Name = dto.Name,
                Comments = dto.Comments,
                Date = dto.Date,
                Qnty = dto.Qnty,
                CenterId = dto.CenterId,
                RecipientName = dto.RecipientName
            });

            if (updated == null) return NotFound(new { message = "الوارد غير موجود" });

            return Ok(MapToDto(updated));
        }

        [HttpDelete("{id:long}")]
        public async Task<GeneralResponse> Delete(long id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return new GeneralResponse (  false,  "الإيراد غير موجود" );
            //NotFound(new { message = "الإيراد غير موجود" });

            return new GeneralResponse(true, "تم حذف الإيراد بنجاح");
        }

        private static InComeDto MapToDto(InCome i) => new(
            i.Id,
            i.Name,
            i.Comments,
            i.Date,
            i.Qnty,
            i.CenterId,
            i.Center?.Name,
            i.RecipientName
        );
    }
}
