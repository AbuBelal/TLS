using SharedLib.Responses;

namespace APIServerLib.Repositories.Interfaces
{
    public interface IGenericInterface<T>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(long id);
        Task<GeneralResponse> Insert(T item);
        Task<GeneralResponse> Update(T item);
        Task<GeneralResponse> DeleteById(long id);
    }
}