using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IUserApi
    {
        [Get(ApiUrls.User.Profile)]
        Task<UserProfileDto> GetUserProfileAsync();

        [Get(ApiUrls.User.Logout)]
        Task Logout();

        [Get(ApiUrls.User.GetAll)]
        Task<List<ApplicationUser>> GetAll();

        [Get(ApiUrls.User.GetAllWithRoles)]
        Task<List<UserWithRoles?>> GetAllWithRoles();

        [Get(ApiUrls.User.GetUserRole)]
        Task<List<string>?> GetUserRole(string id);

        [Get(ApiUrls.User.GetById)]
        Task<ApplicationUser> GetById(string id);

        [Get(ApiUrls.User.GetByEmail)]
        Task<ApplicationUser> GetByEmail(string Email);

        [Post(ApiUrls.User.Insert)]
        Task<GeneralResponse> Insert([Body] AdminUserInputModel user);

        [Put(ApiUrls.User.Update)]
        Task<GeneralResponse> Update(ProfileInputModel Profile);

        [Post(ApiUrls.User.LogIn)]
        Task<GeneralResponse> LogIn(LoginModel loginmodel);

        [Delete(ApiUrls.User.DeleteById)]
        Task<GeneralResponse> DeleteById(string id);

        [Post(ApiUrls.User.ChangePassword)]
        Task<GeneralResponse> ChangePassword([Body] PasswordInputModel passwordInputModel);
        [Post(ApiUrls.User.ResetPassword)]
        Task<GeneralResponse> ResetPassword([Body] PasswordInputModel passwordInputModel);
    }
}
