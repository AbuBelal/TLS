using SharedLib.Responses;

namespace TLSClientSharedLib.Services.Interfaces
{
    public interface IGenericServiceInterface<T>
    {
        Task<List<T>> GetAll(string baseUrl);
        Task<T> GetById(long id, string baseUrl);
        Task<GeneralResponse> Insert(T item, string baseUrl);
        Task<GeneralResponse> Update(T item, string baseUrl);
        Task<GeneralResponse> DeleteById(long id, string baseUrl);
    }
}