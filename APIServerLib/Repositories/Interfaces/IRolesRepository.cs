using Microsoft.AspNetCore.Identity;
using SharedLib.Entities;
using SharedLib.Responses;
using Microsoft.AspNetCore.Identity;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IRolesRepository : IGenericInterface<IdentityRole>
    {
        // أضف هنا أي دوال خاصة بـ Center إذا لزم الأمر
        Task<IdentityRole> GetById(string id);
        Task<GeneralResponse> DeleteById(string id);
    }
}