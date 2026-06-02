using SharedLib.DTOs;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IBackupService
    {
        Task<List<DatabaseInfo>> GetAvailableDatabasesAsync();
        Task<List<BackupRecord>> GetBackupHistoryAsync();
        Task<int> GetDatabaseSizeMBAsync(string dbName);
        Task<BackupRecord> CreateBackupAsync(BackupRequest request,
                                                         CancellationToken ct);
        Task<(byte[] Bytes, string FileName)> GetBackupBytesAsync(long backupId);
        Task DeleteBackupAsync(long backupId);
    }
}
