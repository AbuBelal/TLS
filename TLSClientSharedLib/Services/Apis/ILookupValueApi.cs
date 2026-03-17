// TLSClientSharedLib\Services\Apis\ILookupValueApi.cs
using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface ILookupValueApi
    {
        [Get(ApiUrls.LookupValue.GetAll)]
        Task<List<LookupValue>> GetAll();

        [Get(ApiUrls.LookupValue.GetById)]
        Task<LookupValue> GetById(long id);

        [Post(ApiUrls.LookupValue.Insert)]
        Task<GeneralResponse> Insert([Body] LookupValue lookupValue);

        [Put(ApiUrls.LookupValue.Update)]
        Task<GeneralResponse> Update([Body] LookupValue lookupValue);

        [Delete(ApiUrls.LookupValue.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);
    }
}