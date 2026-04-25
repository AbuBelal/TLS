using SharedLib.Entities;
using SharedLib.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IDailyReportRepository
    {
        Task <List<DailyReport>> GetTodayDailyReportAsync ();
        Task <List<DailyReport>> GetDailyReportFromDateAsync (DateOnly FromDate);
        Task <List<DailyReport>> GetDailyReportForDateAsync (DateOnly date);
        Task<GeneralResponse> UpdateDailyReportAsync (DailyReport dailyreport);
        Task<GeneralResponse> DeleteDailyReportAsync (long Id);
        Task<decimal> GetBuildingTotalDistAsync(string? BuildingId = null);


    }
}
