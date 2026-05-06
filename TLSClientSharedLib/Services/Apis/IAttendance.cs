using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;
using SharedLib.DTOs;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IAttendanceApi
    {
        [Post(ApiUrls.Attendance.GetAttendancesAvg)]
        Task<List<DailyAttendance>> GetAttendancesAvg(AttendanceRequest request);

        [Post(ApiUrls.Attendance.GetAttendancesAllCentersAvg)]
        Task<List<AllCentersDailyAttendance>> GetAttendancesAllCentersAvg(AttendanceRequest request);
    }
}