// المسار: APIServerLib\Repositories\Interfaces\IEmployeeRepository.cs
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericInterface<Employee>
    {
        // أضف هنا أي دوال خاصة بـ Employee إذا لزم الأمر
        Task<int> GetCenterEmployeesCountAsync(long CenterId);
        Task<EmployeePaginatedResponse> GetPaginatedEmployesAsync(EmployeeFilterRequest request, long CenterId = 0);
        Task<EmployeeUpsertDto?> GetByCivilId(string CivilId);
        Task<EmployeeUpsertDto?> GetByEmpId(string EmpId);
        Task<GeneralResponse> AddEmployeeWithCenter(Employee employee, long centerid);
        // ── جديد: للتصدير ──────────────────────────────────────────

        /// <summary>تصدير الموظفين المفلترين بدون pagination</summary>
        Task<List<EmployeeListItemDto>> GetFilteredForExportAsync(EmployeeFilterRequest request, long centerId);

        /// <summary>تصدير جميع موظفي المركز بدون فلاتر</summary>
        Task<List<EmployeeListItemDto>> GetAllByCenterAsync(long centerId);
        Task<List<Employee>> GetAllManagers();
        Task<Employee> GetCenterManagers(long centerId);
        Task<Employee?> IsCivilIdDuplicateAsync(EmployeeDuplicateCheckRequest request);
        Task<Employee?> IsEmpIdDuplicateAsync(EmployeeDuplicateCheckRequest request);
        Task<GeneralResponse> UpdateWithCenter(EmployeeUpsertDto item);
        Task<Employee?> IsEmployeeDuplicateAsync(EmployeeDuplicateCheckRequest request);
    }
}