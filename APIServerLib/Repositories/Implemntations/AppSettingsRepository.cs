using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    public class AppSettingsRepository(ApplicationDbContext context) : IAppSettingsRepository
    {
        public async Task<IEnumerable<AppSetting>> GetAllAsync()
        {
            return await context.AppSetting.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<AppSetting>> GetByCategoryAsync(string category)
        {
            return await context.AppSetting
                .AsNoTracking()
                .Where(s => s.Category == category)
                .ToListAsync();
        }

        public async Task<AppSetting?> GetByKeyAsync(string key)
        {
            return await context.AppSetting
                .FirstOrDefaultAsync(s => s.SettingKey == key);
        }

        public async Task<AppSetting> AddAsync(AppSetting setting)
        {
            // استخدام GUID V7 المدعوم في .NET 10
            if (setting.Id == Guid.Empty)
            {
                setting.Id = Guid.CreateVersion7();
            }

            setting.LastModified = DateTime.UtcNow;
            await context.AppSetting.AddAsync(setting);
            await context.SaveChangesAsync();
            return setting;
        }

        public async Task UpdateAsync(AppSetting setting)
        {
            var existingSetting = await context.AppSetting.FindAsync(setting.Id);
            if (existingSetting != null)
            {
                existingSetting.SettingValueStr = setting.SettingValueStr;
                existingSetting.SettingValueBool = setting.SettingValueBool;
                existingSetting.Category = setting.Category;
                existingSetting.LastModified = DateTime.UtcNow;

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var setting = await context.AppSetting.FindAsync(id);
            if (setting != null)
            {
                context.AppSetting.Remove(setting);
                await context.SaveChangesAsync();
            }
        }
    }
}
