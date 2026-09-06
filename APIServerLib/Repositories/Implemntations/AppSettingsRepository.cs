using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedLib.Entities;
using SharedLib.Fixed;
using SharedLib.Responses;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    public class AppSettingsRepository(ApplicationDbContext context) : IAppSettingsRepository
    {
        public async Task<IEnumerable<AppSetting>> GetAllAsync()
        {
            await AddRequiredKeys();
            return await context.AppSettings.AsNoTracking().OrderBy(x=>x.SortOrder).ToListAsync();
        }

        public async Task<IEnumerable<AppSetting>> GetByCategoryAsync(string category)
        {
            return await context.AppSettings
                .AsNoTracking()
                .Where(s => s.Category == category)
                .ToListAsync();
        }

        public async Task<AppSetting?> GetByKeyAsync(string key)
        {
            return await context.AppSettings
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
            await context.AppSettings.AddAsync(setting);
            await context.SaveChangesAsync();
            return setting;
        }

        public async Task<GeneralResponse> UpdateAsync(AppSetting setting)
        {
            var existingSetting = await context.AppSettings.FindAsync(setting.Id);
            if (existingSetting != null)
            {
                existingSetting.SettingValueStr = setting.SettingValueStr;
                existingSetting.SettingValueBool = setting.SettingValueBool;
                existingSetting.Category = setting.Category;
                existingSetting.SortOrder = setting.SortOrder;
                existingSetting.LastModified = DateTime.UtcNow;

                await context.SaveChangesAsync();
            }
            return new GeneralResponse(true, "تم عملية الحفظ بنجاح");
        }

        public async Task DeleteAsync(Guid id)
        {
            var setting = await context.AppSettings.FindAsync(id);
            if (setting != null)
            {
                context.AppSettings.Remove(setting);
                await context.SaveChangesAsync();
            }
        }

        private async Task AddRequiredKeys()
        {
            List<AppSetting> requiredSettings = new List<AppSetting>();

            //var fields = typeof(RequiredAppSettings).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            //foreach (var field in fields)
            //{
            //    string propertyName=string.Empty;
            //    string propertyValue= string.Empty;

            //    if (field.IsLiteral && !field.IsInitOnly)
            //    {
            //         propertyName = field.Name; 
            //         propertyValue = (string)field.GetValue(null); 

            //        //Console.WriteLine($"Key: {propertyName}, Value: {propertyValue}");
            //    }

            //    if (!context.AppSettings.Any(x => x.SettingKey == propertyValue))
            //    {
            //        var newSetting = new AppSetting
            //        {
            //            SettingKey = propertyValue,
            //            SettingValueStr = "",
            //            Category = SharedLib.Fixed.AppSettingsCategories.AreaData
            //        };
            //        requiredSettings.Add(newSetting);
            //    }
            //}
            ////////////
            foreach (var item in RequiredAppSettings.SettingsWithCategories)
            {
                if (!context.AppSettings.Any(x=>x.SettingKey == item.Key))
                {
                    var newSetting = new AppSetting
                    {
                        SettingKey = item.Key,           // نأخذه من مفتاح القاموس
                        Category = item.Value.Category,  // نأخذه من الكائن البسيط
                        SettingValueStr = item.Value.SettingType == 1 ? "" : null,
                        SettingValueBool = item.Value.SettingType==2 ? false : null,
                        SortOrder = item.Value.SortOrder,
                       
                    };
                    requiredSettings.Add(newSetting);
                }
            }

            if (requiredSettings.Count > 0)
            {
                await context.AppSettings.AddRangeAsync(requiredSettings);
                await context.SaveChangesAsync();
            }
        }
    }
}
