// المسار: APIServerLib\Repositories\Interfaces\IEmployeeRepository.cs
using SharedLib.DTOs;
using SharedLib.Entities;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericInterface<Employee>
    {
        // أضف هنا أي دوال خاصة بـ Employee إذا لزم الأمر
        Task<int> GetCenterEmployeesCountAsync(long CenterId);
        Task<EmployeePaginatedResponse> GetPaginatedEmployesAsync(EmployeeFilterRequest request, long CenterId = 0);
        Task<EmployeeUpsertDto?> GetByCivilId(string CivilId);
        Task<EmployeeUpsertDto?> GetByEmpId(string EmpId);
    }
}