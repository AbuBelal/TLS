using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedLib.DTOs;
using SharedLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
namespace APIServerLib.Repositories.Implemntations
{
    public class BackupService(ApplicationDbContext db, IHostEnvironment env,
                           IConfiguration config,
                           ILogger<BackupService> logger) : IBackupService
{
    private readonly string _conn = config.GetConnectionString(SharedLib.Fixed.SystemSettings.SelectedAreaEn)!;

    public async Task<BackupRecord> CreateBackupAsync(BackupRequest request,
                                                       CancellationToken ct)
    {
        Directory.CreateDirectory(request.SavePath);

        var fileName = $"{request.BackupName}.{request.Format}";
            //var fullPath = Path.Combine(request.SavePath, fileName);
            var fullPath = Path.Combine(env.ContentRootPath, $"Backups\\{fileName}");

            var sql = $"""
            BACKUP DATABASE [{request.DatabaseName}]
            TO DISK = N'{fullPath}'
            WITH
                {(request.Compress ? "COMPRESSION," : "")}
                FORMAT,
                NAME = N'{request.BackupName}',
                STATS = 10
            """;

        await using var conn = new SqlConnection(_conn);
        await conn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 3600 };
        await cmd.ExecuteNonQueryAsync(ct);
       Console.WriteLine(fullPath);
        if (request.VerifyAfter)
        {
            var verifySql = $"RESTORE VERIFYONLY FROM DISK = N'{fullPath}'";
            await using var vCmd = new SqlCommand(verifySql, conn) { CommandTimeout = 600 };
            await vCmd.ExecuteNonQueryAsync(ct);
        }

        var fileInfo = new FileInfo(fullPath);
        var record = new BackupRecord
        {
            FileName     = fileName,
            FilePath     = fullPath,
            DatabaseName = request.DatabaseName,
            BackupType   = request.BackupType,
            Format       = request.Format,
            SizeMB       = (int)(fileInfo.Length / 1024 / 1024),
            IsCompressed = request.Compress,
            IsEncrypted  = request.Encrypt,
            IsVerified   = request.VerifyAfter,
            Status       = BackupStatus.Success,
            CreatedAt    = DateTime.Now,
        };

        db.BackupRecords.Add(record);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Backup created: {Path}", fullPath);
        return record;
    }

    public async Task<List<BackupRecord>> GetBackupHistoryAsync()
        => await db.BackupRecords
                   .Where(b => b.DeletedAt == null)
                   .OrderByDescending(b => b.CreatedAt)
                   .Take(50)
                   .ToListAsync();

    public async Task<int> GetDatabaseSizeMBAsync(string dbName)
    {
        var sql = $"""
            SELECT SUM(size * 8 / 1024) 
            FROM sys.master_files
            WHERE database_id = DB_ID('{dbName}')
            """;
        await using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        return (int)(await cmd.ExecuteScalarAsync() ?? 0);
    }

    public async Task<(byte[] Bytes, string FileName)> GetBackupBytesAsync(long id)
    {
        var record = await db.BackupRecords.FindAsync(id)
            ?? throw new FileNotFoundException();
        var bytes = await File.ReadAllBytesAsync(record.FilePath);
        return (bytes, record.FileName);
    }

    public async Task DeleteBackupAsync(long id)
    {
        var record = await db.BackupRecords.FindAsync(id);
        if (record is null) return;
        if (File.Exists(record.FilePath)) File.Delete(record.FilePath);
        record.DeletedAt = DateTime.Now; // Soft Delete
        await db.SaveChangesAsync();
    }

        //public async Task<List<DatabaseInfo>> GetAvailableDatabasesAsync()
        //    => await Task.FromResult(new List<DatabaseInfo>
        //    {
        //       new("TempEduNorth_DB", "TempEduNorth — شمال غزة"),
        //        new("TempEduMid_DB",   "TempEduMid — غرب الوسطى"),
        //    });

        public async Task<List<DatabaseInfo>> GetAvailableDatabasesAsync()
        {
            // أسماء عربية مخصصة — أضف أو عدّل حسب قواعد بياناتك
            var arabicNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["TempEduNorth_DB"] = "TempEduNorth — شمال غزة",
                ["TempEduMid_DB"] = "TempEduMid — غرب الوسطى",
            };

            const string sql = """
                SELECT name
                FROM sys.databases
                WHERE state_desc = 'ONLINE'
                  AND name NOT IN ('master', 'tempdb', 'model', 'msdb')
                ORDER BY name
                """;

            var result = new List<DatabaseInfo>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var name = reader.GetString(0);
                result.Add(new DatabaseInfo
                (
                     name,
                    arabicNames.TryGetValue(name, out var arabic) ? arabic : name
                ));
            }

            return result;
        }
    }
}
