using Blazored.LocalStorage;
using SharedLib.DTOs;
using SharedLib.Entities;
using System.Text;
using System.Text.Json;

namespace TLSWeb.Services
{
    public interface IUserSettingsService
    {
        Task SaveUserSettingsAsync(List<AppSetting> data);
        Task<List<AppSetting>?> GetUserSettingsAsync();
        Task ClearUserSettingsAsync();
    }

    public class UserSettingservice : IUserSettingsService
    {
        private readonly ILocalStorageService _localStorage;
        private const string StorageKey = "current_u_Settings";

        // مفتاح تشفير خاص بك لتطبيق عملية التشفير الرقمي
        private const string EncryptionSecret = "TLSWebSystemS@j@Bel@l@na$4691980!";

        public UserSettingservice(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveUserSettingsAsync(List<AppSetting> data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);

                // تشفير الـ JSON إلى نص مشفر متوافق تماماً مع Blazor WASM
                var encryptedData = Encrypt(json, EncryptionSecret);

                await _localStorage.SetItemAsync(StorageKey, encryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving UserData: {ex.Message}");
                throw;
            }
        }

        public async Task<List<AppSetting>?> GetUserSettingsAsync()
        {
            try
            {
                var encryptedData = await _localStorage.GetItemAsync<string>(StorageKey);
                if (string.IsNullOrEmpty(encryptedData)) return null;

                // فك تشفير البيانات
                var decryptedJson = Decrypt(encryptedData, EncryptionSecret);

                return JsonSerializer.Deserialize<List<AppSetting>>(decryptedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading UserData: {ex.Message}");
                return null;
            }
        }

        public async Task ClearUserSettingsAsync()
        {
            await _localStorage.RemoveItemAsync(StorageKey);
        }

        #region Lightweight WASM Compatible Encryption

        // خوارزمية تشفير متوافقة 100% مع WebAssembly تضمن حماية البيانات من القراءة في DevTools
        private string Encrypt(string plainText, string key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var result = new byte[plainBytes.Length];

            for (int i = 0; i < plainBytes.Length; i++)
            {
                result[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(result);
        }

        private string Decrypt(string cipherText, string key)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var result = new byte[cipherBytes.Length];

            for (int i = 0; i < cipherBytes.Length; i++)
            {
                result[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(result);
        }

        #endregion
    }
}