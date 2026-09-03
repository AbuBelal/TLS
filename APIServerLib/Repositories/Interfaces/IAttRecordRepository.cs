using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IAttRecordRepository 
    {
        Task<int> GenerateMonthlyAttendanceAsync(int year, int month, long? holidayCode);
        Task<List<AttendanceRecord>> GetAttendanceByCenterAsync(long centerId, int year, int month);
        Task<bool> UpdateAttendanceRecordsAsync(List<AttendanceRecord> records);
        Task LockAttendanceRecordsAsync(int year, int month ,bool Lock = true);
        Task<GeneralResponse> DeleteEmployeeAttendanceRecordsAsync(AttRecoRequest request);
        Task<List<AttRecSummery>> GetAttendanceSummaryAsync(int year, int month);
    }
}