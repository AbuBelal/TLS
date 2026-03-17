using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;
using SharedLib.DTOs;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IAccountApi
    {
        [Post(ApiUrls.Auth.Register)]
        Task<GeneralResponse> Register(string email, string password);

        [Post(ApiUrls.Auth.Login)]
        Task<HttpResponseMessage> Login(LoginModel credentials);

        [Get(ApiUrls.Auth.UserInfo)]
        Task<UserInfo> GetUserInfoAsync();

        [Post(ApiUrls.Auth.Logout)]
        Task<GeneralResponse> Logout(StringContent empty);

        // أضف هنا أي عمليات أخرى مثل تغيير كلمة المرور أو استرجاعها إذا لزم الأمر
    }
}