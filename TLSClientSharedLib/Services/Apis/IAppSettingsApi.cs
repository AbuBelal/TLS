using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;
using SharedLib.DTOs;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IAppSettingsApi
    {
        [Get(ApiUrls.AppSettingsApiUls.GetAll)]
        Task<List<AppSetting>> GetAll();
        [Get(ApiUrls.AppSettingsApiUls.GetByKey)]
        Task<AppSetting> GetByKey(string key);
        [Get(ApiUrls.AppSettingsApiUls.GetByCategory)]
        Task<AppSetting> GetByCategory(string category);
        [Post(ApiUrls.AppSettingsApiUls.Add)]
        Task<AppSetting> Add(AppSetting appSetting);
        [Put(ApiUrls.AppSettingsApiUls.Update)]
        Task<AppSetting> Update(AppSetting appSetting);
        [Delete(ApiUrls.AppSettingsApiUls.DeleteById)]
        Task DeleteById(string id);
    }
}