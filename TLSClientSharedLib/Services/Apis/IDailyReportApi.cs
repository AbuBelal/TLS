using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IDailyReportApi
    {
        [Get(ApiUrls.Reports.DailyReport)]
        Task<List<DailyReport>> GetDailyReport();

        [Get(ApiUrls.Reports.DailyReportForDate)]
        Task<List<DailyReport>> GetDailyReportForDate([Query] DateOnly date);

        [Post(ApiUrls.Reports.UpdateDailyReport)]
        Task<GeneralResponse> UpdateDailyReport([Body] DailyReport dailyReport);

        /// <summary>تصدير التقارير اليومية (POST لأن الفلاتر في الـ body)</summary>
        [Post(ApiUrls.Reports.ExportDailyReport)]
        Task<HttpResponseMessage> Export( DateOnly date);

        [Get(ApiUrls.Reports.GetBuildingTotalDist)]
        Task<decimal> GetBuildingTotalDist(string? BuildingId = null);

    }
}
