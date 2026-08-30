using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IAppSettingsRepository
    {
        Task<IEnumerable<AppSetting>> GetAllAsync();
        Task<IEnumerable<AppSetting>> GetByCategoryAsync(string category);
        Task<AppSetting?> GetByKeyAsync(string key);
        Task<AppSetting> AddAsync(AppSetting setting);
        Task UpdateAsync(AppSetting setting);
        Task DeleteAsync(Guid id);
    }
}
