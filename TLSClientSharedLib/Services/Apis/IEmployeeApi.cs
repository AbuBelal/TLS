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

        [Post(ApiUrls.Employee.Insert)]
        Task<GeneralResponse> Insert([Body] EmployeeUpsertDto employee);

        [Put(ApiUrls.Employee.Update)]
        Task<GeneralResponse> Update([Body] EmployeeUpsertDto employee);

        [Delete(ApiUrls.Employee.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);

        [Get(ApiUrls.Employee.EmployeesCenterCount)]
        Task<int> GetEmployeesCountByCenterId(long id);

        [Post(ApiUrls.Employee.Paginated)]
        Task<EmployeePaginatedResponse> GetPaginated(EmployeeFilterRequest request);
    }
}