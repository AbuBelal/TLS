// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServerLib/Repositories/Interfaces/IAdminDashboardRepository.cs ║
// ╚══════════════════════════════════════════════════════════════╝

using SharedLib.DTOs;

namespace APIServerLib.Repositories.Interfaces;

public interface IAdminDashboardRepository
{
    Task<AdminDashboardDto> GetAdminDashboardAsync();
}
