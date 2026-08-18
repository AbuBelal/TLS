using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IAttendanceRecordApi
    {
        [Get(ApiUrls.AttendanceRecord.GetGenerateMonthlyAttendance)]
        Task<List<AttendanceRecord>> GetGenerateMonthlyAttendance(int year, int month);

        [Put(ApiUrls.AttendanceRecord.Update)]
        Task<GeneralResponse> Update(List<AttendanceRecord> records);

    }
}