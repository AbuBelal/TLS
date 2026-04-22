using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Implemntations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            return auditLog;
        }

        public async Task<List<AuditLog>> GetAll()
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByUserId(string userId)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByEntityType(string entityType)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<AuditLogDto>> GetPaginated(AuditLogFilterRequest request)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
                query = query.Where(a =>
                    a.UserName!.Contains(request.SearchText) ||
                    a.Details!.Contains(request.SearchText));

            if (!string.IsNullOrWhiteSpace(request.Action))
                query = query.Where(a => a.Action==request.Action);

            if (!string.IsNullOrWhiteSpace(request.Details))
                query = query.Where(a => a.Details.Contains(request.Details));

            if (!string.IsNullOrWhiteSpace(request.EntityType))
                query = query.Where(a => a.EntityType == request.EntityType);

            if (!string.IsNullOrWhiteSpace(request.UserId))
                query = query.Where(a => a.UserId == request.UserId);

            if (request.FromDate.HasValue)
                query = query.Where(a => a.Timestamp >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(a => a.Timestamp <= request.ToDate.Value);

            var total = await query.CountAsync();

            var orderedQuery = request.SortDescending
               ? query.OrderByDescending(a => a.Timestamp)
                : query.OrderBy(a => a.Timestamp);

            var items = await orderedQuery
              .Skip((request.PageNumber - 1) * request.PageSize)
              .Take(request.PageSize)
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.UserName,
                    Action = a.Action,
                    EntityType = a.EntityType,
                    EntityId = a.EntityId,
                    Details = a.Details,
                    IpAddress = a.IpAddress,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();

            return new PaginatedResponse<AuditLogDto>
            {
                Items = items,
                TotalCount = total,
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
