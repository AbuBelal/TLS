using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IAttendanceRecordApi
    {
        [Get(ApiUrls.AttendanceRecord.GetGeneratedMonthlyAttendance)]
        Task<List<AttendanceRecord>> GetGeneratedMonthlyAttendance(int year, int month);

        [Put(ApiUrls.AttendanceRecord.Update)]
        Task<GeneralResponse> Update(List<AttendanceRecord> records);

        [Post(ApiUrls.AttendanceRecord.GenerateMonthlyAttendance)]
        Task<GeneralResponse> GenrateMonthlyAttendance(GenerateAttendanceRequest request);

        [Post(ApiUrls.AttendanceRecord.LockMonthlyAttendance)]
        Task<GeneralResponse> LockMonthlyAttendance(GenerateAttendanceRequest request);

        [Post(ApiUrls.AttendanceRecord.Export)]
        Task<HttpResponseMessage> Export(GenerateAttendanceRequest request);

    }
}