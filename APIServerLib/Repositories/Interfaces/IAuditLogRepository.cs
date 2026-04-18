using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task<List<AuditLog>> GetAll();
        Task<List<AuditLog>> GetByUserId(string userId);
        Task<List<AuditLog>> GetByEntityType(string entityType);
        Task<PaginatedResponse<AuditLogDto>> GetPaginated(AuditLogFilterRequest request);
    }
}
