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
    public interface IRolesApi
    {
        [Get(ApiUrls.Roles.GetAll)]
        Task<List<string>> GetAll();

        [Post(ApiUrls.Roles.Insert)]
        Task<GeneralResponse> Insert([Body] string RoleName);

        [Put(ApiUrls.Roles.Update)]
        Task<GeneralResponse> Update([Body] string id , string RoleName);

        [Delete(ApiUrls.Roles.DeleteById)]
        Task<GeneralResponse> DeleteById(string id);
    }
}
