// TLSClientSharedLib\Services\Apis\IStdCenterApi.cs
using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IStdCenterApi
    {
        [Get(ApiUrls.StdCenter.GetAll)]
        Task<List<StdCenter>> GetAll();

        [Get(ApiUrls.StdCenter.GetById)]
        Task<StdCenter> GetById(long id);

        [Post(ApiUrls.StdCenter.Insert)]
        Task<GeneralResponse> Insert([Body] StdCenter stdCenter);

        [Put(ApiUrls.StdCenter.Update)]
        Task<GeneralResponse> Update([Body] StdCenter stdCenter);

        [Delete(ApiUrls.StdCenter.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);
    }
}