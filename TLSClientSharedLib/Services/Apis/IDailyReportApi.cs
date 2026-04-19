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

    }
}
