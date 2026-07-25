using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IWReportApi
    {
        [Get(ApiUrls.WReport.GetAll)]
        Task<List<WReport>> GetAll();

        [Get(ApiUrls.WReport.GetById)]
        Task<WReport> GetById(long id);

        [Post(ApiUrls.WReport.Insert)]
        Task<GeneralResponse> Insert([Body] WReport report);

        [Put(ApiUrls.WReport.Update)]
        Task<GeneralResponse> Update([Body] WReport report);

        [Delete(ApiUrls.WReport.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);
    }
}