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


        // أضف هنا أي عمليات أخرى مثل تغيير كلمة المرور أو استرجاعها إذا لزم الأمر
    }
}