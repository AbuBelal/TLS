// TLSClientSharedLib\Services\Apis\IEmpCenterApi.cs
using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IEmpCenterApi
    {
        [Get(ApiUrls.EmpCenter.GetAll)]
        Task<List<EmpCenter>> GetAll();

        [Get(ApiUrls.EmpCenter.GetById)]
        Task<EmpCenter> GetById(long id);

        [Post(ApiUrls.EmpCenter.Insert)]
        Task<GeneralResponse> Insert([Body] EmpCenter empCenter);

        [Put(ApiUrls.EmpCenter.Update)]
        Task<GeneralResponse> Update([Body] EmpCenter empCenter);

        [Delete(ApiUrls.EmpCenter.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);
    }
}