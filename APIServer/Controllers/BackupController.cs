using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;

namespace APIServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class BackupController(IBackupService backupService,
                               ILogger<BackupController> logger) : ControllerBase
{
    // GET api/backup/databases
    [HttpGet("databases")]
    public async Task<IActionResult> GetDatabases()
    {
        var list = await backupService.GetAvailableDatabasesAsync();
        return Ok(list);
    }

    // GET api/backup/history
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var list = await backupService.GetBackupHistoryAsync();
        return Ok(list);
    }

    // GET api/backup/dbsize/{dbName}
    [HttpGet("dbsize/{dbName}")]
    public async Task<IActionResult> GetDbSize(string dbName)
    {
        var size = await backupService.GetDatabaseSizeMBAsync(dbName);
        return Ok(new { sizeMB = size });
    }

    // POST api/backup/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateBackup([FromBody] BackupRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var record = await backupService.CreateBackupAsync(request, CancellationToken.None);
            return Ok(record);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Backup failed for {DatabaseName}", request.DatabaseName);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET api/backup/download/{id}
    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var (bytes, fileName) = await backupService.GetBackupBytesAsync(id);
            return File(bytes, "application/octet-stream", fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { message = "الملف غير موجود" });
        }
    }

    // DELETE api/backup/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await backupService.DeleteBackupAsync(id);
        return NoContent();
    }
}