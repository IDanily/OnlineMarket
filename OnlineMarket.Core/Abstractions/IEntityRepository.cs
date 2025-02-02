namespace OnlineMarket.Core.Abstractions
{
    public interface IEntityRepository<T> where T : class
    {
        Task<int> Create(T entity);
        Task<int> Delete(int id);
        Task<List<T>> GetAll();
        Task<T> Get(int id);
        Task<int> Update(T entity);
    }
}