// TLSClientSharedLib\Services\Apis\IStudentApi.cs
using Refit;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSClientSharedLib.Helpers;

namespace TLSClientSharedLib.Services.Apis
{
    public interface IStudentApi
    {
        [Get(ApiUrls.Student.GetAll)]
        Task<List<Student>> GetAll();

        [Get(ApiUrls.Student.GetById)]
        Task<Student> GetById(long id);

        [Post(ApiUrls.Student.Insert)]
        Task<GeneralResponse> Insert([Body] Student student);

        [Put(ApiUrls.Student.Update)]
        Task<GeneralResponse> Update([Body] Student student);

        [Delete(ApiUrls.Student.DeleteById)]
        Task<GeneralResponse> DeleteById(long id);

        [Get(ApiUrls.Student.StudentsCenterCount)]
        Task<int> GetStudentCountByCenterId(long id);
    }
}