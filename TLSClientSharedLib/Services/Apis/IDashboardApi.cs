using Refit;
using SharedLib.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IDashboardApi
    {
        [Get(ApiUrls.DashBoard.Get)]
        Task<CenterDashboardDto?> Get();
    }
}
