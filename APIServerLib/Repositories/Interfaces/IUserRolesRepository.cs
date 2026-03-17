using Microsoft.AspNetCore.Identity;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IUserRolesRepository : IGenericInterface<IdentityUserRole<string>>
    {
        // أضف هنا أي دوال خاصة بـ Center إذا لزم الأمر
        Task<IdentityRole> GetById(string id);
        Task<List<IdentityRole>> GetRolesForUser(long id);
        Task<GeneralResponse> DeleteById(string id);
    }
}