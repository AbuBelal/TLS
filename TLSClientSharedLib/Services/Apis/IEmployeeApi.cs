// TLSClientSharedLib\Services\Apis\IEmployeeApi.cs
using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IEmployeeApi
    {
        [Get(ApiUrls.Employee.GetAll)]
        Task<List<Employee>> GetAll();

        [Get(ApiUrls.Employee.GetById)]
        Task<Employee> GetById(long id);

        [Get(ApiUrls.Employee.GetByCivilId)]
        Task<EmployeeUpsertDto?> GetByCivilId(string CivilId);

        [Get(ApiUrls.Employee.GetByEmpId)]
        Task<EmployeeUpsertDto?> GetByEmpId(string EmpId);

        [Post(ApiUrls.Employee.Insert)]
        Task<GeneralResponse> Insert([Body] EmployeeUpsertDto employee);

        [Post(ApiUrls.Employee.AddWithCenter)]
        Task<GeneralResponse> AddWithCenter([Body] EmployeeWithCenter employee);

        [Put(ApiUrls.Employee.Update)]
        Task<GeneralResponse> Update([Body] EmployeeUpsertDto employee);

        [Delete(ApiUrls.Employee.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);

        [Get(ApiUrls.Employee.EmployeesCenterCount)]
        Task<int> GetEmployeesCountByCenterId(long id);

        [Post(ApiUrls.Employee.Paginated)]
        Task<EmployeePaginatedResponse> GetPaginated(EmployeeFilterRequest request);


        // ── جديد: تصدير ────────────────────────────────────────────

        /// <summary>تصدير الموظفين المفلترين (POST لأن الفلاتر في الـ body)</summary>
        [Post(ApiUrls.Employee.ExportFiltered)]
        Task<HttpResponseMessage> ExportFiltered([Body] EmployeeFilterRequest request);

        /// <summary>تصدير جميع موظفي المركز</summary>
        [Get(ApiUrls.Employee.ExportAll)]
        Task<HttpResponseMessage> ExportAll();
        [Get(ApiUrls.Employee.Managers)]
        Task<List<Employee>> GetAllManagers();

        [Get(ApiUrls.Employee.CenterManager)]
        Task<Employee> GetCenterManager(long centerId);
        [Post(ApiUrls.Employee.IsCivilIdDuplicate)]
        Task<Employee?> IsCivilIdDuplicate(EmployeeDuplicateCheckRequest request);
        [Post(ApiUrls.Employee.IsEmpIdDuplicate)]
        Task<Employee?> IsEmpIdDuplicate(EmployeeDuplicateCheckRequest request);
    }
}