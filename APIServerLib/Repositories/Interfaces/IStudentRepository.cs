// APIServerLib\Repositories\Interfaces\IStudentRepository.cs
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IStudentRepository : IGenericInterface<Student>
    {
        // أضف هنا أي دوال خاصة بـ Student إذا لزم الأمر
        Task<int> GetCenterStudentsCountAsync(long CenterId);
        Task<GeneralResponse> AddStudentWithCenter(Student student , long centerid);
        Task<GeneralResponse> UpdateStudentWithCenter(Student student , long centerid);
        Task<PaginatedResponse<StudentDto>> GetPaginatedStudentsAsync(StudentFilterRequest request, long CenterId=0);
        // ── جديد: للتصدير ──────────────────────────────────────────

        /// <summary>
        /// جلب الطلاب المفلترة بدون pagination — لتصدير المعروض
        /// </summary>
        Task<List<Student>> GetFilteredForExportAsync(StudentFilterRequest request, long centerId);

        /// <summary>
        /// جلب جميع طلاب المركز بدون فلاتر — للتصدير الكامل
        /// </summary>
        Task<List<Student>> GetAllByCenterAsync(long centerId);
    }
}