// TLSClientSharedLib\Services\Apis\ICenterApi.cs
using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface ICenterApi
    {
        [Get(ApiUrls.Center.GetAll)]
        Task<List<Center>> GetAll();

        [Get(ApiUrls.Center.GetById)]
        Task<Center> GetById(long id);

        [Post(ApiUrls.Center.Insert)]
        Task<GeneralResponse> Insert([Body] Center center);

        [Put(ApiUrls.Center.Update)]
        Task<GeneralResponse> Update([Body] CenterUpsertDto center);

        [Delete(ApiUrls.Center.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);

        [Get(ApiUrls.Center.MyCenter)]
        Task<Center> GetMyCenter();

        [Put(ApiUrls.Center.MyCenter)]
        Task<GeneralResponse> UpdateMyCenter([Body] CenterUpsertDto dto);
    }
}