namespace OnlineMarket.Core.Abstractions
{
    public interface IEntityService<T> where T : class
    {
        Task<int> Create(T entity);
        Task<int> Delete(int id);
        Task<List<T>> GetAll();
        Task<T> Get(int id);
        Task<int> Update(T entity);
    }
}