using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IWReportDetailsApi
    {
        [Get(ApiUrls.WReportDetails.GetAll)]
        Task<List<WReportDetail>> GetAll();

        [Get(ApiUrls.WReportDetails.GetById)]
        Task<WReportDetail> GetById(long id);

        [Post(ApiUrls.WReportDetails.Insert)]
        Task<GeneralResponse> Insert([Body] WReportDetail reportDetail);

        [Put(ApiUrls.WReportDetails.Update)]
        Task<GeneralResponse> Update([Body] WReportDetail reportDetail);

        [Delete(ApiUrls.WReportDetails.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);
    }
}