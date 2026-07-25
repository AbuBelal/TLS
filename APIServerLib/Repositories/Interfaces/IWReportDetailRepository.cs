using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IWReportDetailRepository
    {
        Task<IEnumerable<WReportDetail>> GetAllAsync();
        Task<WReportDetail?> GetByIdAsync(long id);
        Task<IEnumerable<WReportDetail>> GetByWReportIdAsync(long wReportId); // دالة إضافية هامة
        Task<WReportDetail> AddAsync(WReportDetail reportDetail);
        Task UpdateAsync(WReportDetail reportDetail);
        Task DeleteAsync(long id);
    }
}
