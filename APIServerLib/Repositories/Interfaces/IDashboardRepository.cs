using SharedLib.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public  interface IDashboardRepository
    {
        Task<CenterDashboardDto?> GetCenterDashboardAsync(string userId);


       
    }
}
