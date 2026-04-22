using Refit;
using SharedLib.DTOs;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IAuditLogApi
    {
        [Get(ApiUrls.AuditLog.GetAll)]
        Task<List<AuditLogDto>> GetAll();

        [Post(ApiUrls.AuditLog.Paginated)]
        Task<PaginatedResponse<AuditLogDto>> GetPaginated([Body] AuditLogFilterRequest request);

        [Get(ApiUrls.AuditLog.GetByUserId)]
        Task<List<AuditLogDto>> GetByUserId(string userId);

        [Get(ApiUrls.AuditLog.GetByEntityType)]
        Task<List<AuditLogDto>> GetByEntityType(string entityType);
    }
}
