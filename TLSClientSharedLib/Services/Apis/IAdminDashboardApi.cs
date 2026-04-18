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
    [Get(ApiUrls.AdminDashBoard.DetailedReport)]
    Task<DetailedCentersReportDto> GetDetailedReport();

    [Get(ApiUrls.AdminDashBoard.ExportDetailedReport)]
    Task<HttpResponseMessage> ExportDetailedReport();

    [Get(ApiUrls.AdminDashBoard.DailyReport)]
    Task<DailyReportDto> GetDailyReport(DateOnly? date = null);

    [Post(ApiUrls.AdminDashBoard.LockDailyReport)]
    Task<HttpResponseMessage> LockDailyReport([Body] DailyReportLockRequest request);
}
