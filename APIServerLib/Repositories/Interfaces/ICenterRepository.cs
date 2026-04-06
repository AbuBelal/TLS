using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    public interface ICenterRepository : IGenericInterface<Center>
    {
        // أضف هنا أي دوال خاصة بـ Center إذا لزم الأمر
        /// <summary>
        /// جلب المركز المرتبط بالمستخدم الحالي عبر Employee → EmpCenter
        /// </summary>
        Task<Center?> GetByUserIdAsync(string userId);

        /// <summary>
        /// تعديل بيانات المركز من خلال DTO (آمن — يتحقق من الملكية)
        /// </summary>
        Task<GeneralResponse> UpdateByUserAsync(CenterUpsertDto dto, string userId);
        Task<GeneralResponse> Update(CenterUpsertDto dto);
    }
}