using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Entities;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AppSettingsController(IAppSettingsRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var settings = await repository.GetAllAsync();
        return Ok(settings);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var setting = await repository.GetByKeyAsync(key);
        if (setting == null) return NotFound();
        return Ok(setting);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var settings = await repository.GetByCategoryAsync(category);
        return Ok(settings);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AppSetting setting)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingSetting = await repository.GetByKeyAsync(setting.SettingKey);
        if (existingSetting != null)
            return BadRequest("يوجد إعداد مسبق بنفس المفتاح.");

        var createdSetting = await repository.AddAsync(setting);
        return CreatedAtAction(nameof(GetByKey), new { key = createdSetting.SettingKey }, createdSetting);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AppSetting setting)
    {
        if (id != setting.Id) return BadRequest("المعرف غير متطابق.");

        await repository.UpdateAsync(setting);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await repository.DeleteAsync(id);
        return NoContent();
    }
}