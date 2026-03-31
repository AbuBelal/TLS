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
        Task<PaginatedResponse<Student>> GetPaginatedStudentsAsync(StudentFilterRequest request, long CenterId=0);
    }
}