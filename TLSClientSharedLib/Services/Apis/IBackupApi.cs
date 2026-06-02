using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IBackupApi
    {
        [Get(ApiUrls.Backup.GetDatabases)]
        Task<List<DatabaseInfo>> GetDatabasesAsync();

        [Get(ApiUrls.Backup.GetHistory)]
        Task<List<BackupRecord>> GetHistoryAsync();

        [Get(ApiUrls.Backup.GetDbSize)]
        Task<DbSizeResponse> GetDbSizeAsync(string dbName);

        [Post(ApiUrls.Backup.Create)]
        Task<BackupRecord> CreateBackupAsync([Body] BackupRequest request);

        [Get(ApiUrls.Backup.Download)]
        Task<HttpContent> DownloadBackupAsync(long id);

        [Delete(ApiUrls.Backup.Delete)]
        Task DeleteBackupAsync(long id);
    }
    public record DbSizeResponse(int SizeMB);

}
