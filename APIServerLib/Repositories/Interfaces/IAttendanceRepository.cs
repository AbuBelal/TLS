using SharedLib.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<List<DailyAttendance>> GetAttendancesAsync(long CenterId, DateOnly From, DateOnly To);
        Task<List<AllCentersDailyAttendance>> GetAttendancesAllCentersAsync(DateOnly From, DateOnly To);
    }
}
