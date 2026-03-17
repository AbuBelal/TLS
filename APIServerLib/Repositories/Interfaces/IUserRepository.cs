using SharedLib.DTOs;
using SharedLib.Responses;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Interfaces
{
    public  interface IUserRepository : IGenericInterface<ApplicationUser>
    {
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<List<UserWithRoles?>> GetAllWithRols();
        Task<ApplicationUser> GetUserByEmail(string Email);
        Task<GeneralResponse> DeleteById(string Id);
        Task<ApplicationUser> GetById(string Id);
        //Task<GeneralResponse> ChangePassword(PasswordInputModel passwordInputModel);
    }
}
