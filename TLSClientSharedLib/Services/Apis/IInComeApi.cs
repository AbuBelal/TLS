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
    public interface IInComeApi
    {
        [Get(ApiUrls.InCome.GetAll)]
        Task<List<InComeDto>> GetAll();
        [Get(ApiUrls.InCome.GetById)]
        Task<InComeDto> GetById(long id);
        [Get(ApiUrls.InCome.GetByCenter)]
        Task<List<InComeDto>> GetByCenter(long centerId);
        [Get(ApiUrls.InCome.GetByDateRange)]
        Task<InComeDto> GetByDateRange(long dateRange);
        [Get(ApiUrls.InCome.GetTotal)]
        Task<decimal> GetTotal();
        [Post(ApiUrls.InCome.Insert)]
        Task<InComeDto> Create(CreateInComeDto dto);

        [Put(ApiUrls.InCome.Update)]
        Task<InComeDto> Update(UpdateInComeDto dto);

        [Delete(ApiUrls.InCome.DeleteById)]
        Task<GeneralResponse> Delete(long id);

        [Get(ApiUrls.InCome.GetBuildingTotal)]
        Task<decimal> GetBuildingTotal(string? BuildingId = null);
    }
}