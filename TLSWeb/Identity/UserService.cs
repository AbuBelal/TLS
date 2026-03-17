using SharedLib.DTOs;
using System.Net.Http.Json;
using TLSClientSharedLib.Services.Apis;

namespace TLSWeb.Identity
{
    public class UserService (IUserApi UserApi) : IUserService
    {
        //private readonly HttpClient _http;

        //public UserService(HttpClient http)
        //{
        //    _http = http;
        //}

        public async Task<UserProfileDto?> GetCurrentUserProfileAsync()
        {
            // استدعاء الـ API لجلب بيانات المستخدم
            return await UserApi.GetUserProfileAsync();
        }
    }
}
