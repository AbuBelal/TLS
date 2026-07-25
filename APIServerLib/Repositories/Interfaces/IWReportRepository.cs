using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IWReportRepository
    {
        Task<IEnumerable<WReport>> GetAllAsync();
        Task<WReport?> GetByIdAsync(long id);
        Task<WReport> AddAsync(WReport report);
        Task UpdateAsync(WReport report);
        Task DeleteAsync(long id);
    }
}
