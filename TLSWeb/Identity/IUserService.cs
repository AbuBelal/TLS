using SharedLib.DTOs;

namespace TLSWeb.Identity
{
    public interface IUserService
    {
        // دالة تجلب بيانات المستخدم الحالي مباشرة من الـ API
        Task<UserProfileDto?> GetCurrentUserProfileAsync();
    }
}
