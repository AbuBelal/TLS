// ╔══════════════════════════════════════════════════════════════╗
// ║  TLSClientSharedLib/Services/Apis/IAdminDashboardApi.cs      ║
// ╚══════════════════════════════════════════════════════════════╝

using Refit;
using SharedLib.DTOs;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis;

public interface IAdminDashboardApi
{
    [Get(ApiUrls.AdminDashBoard.Get)]
    Task<AdminDashboardDto> Get();
}
