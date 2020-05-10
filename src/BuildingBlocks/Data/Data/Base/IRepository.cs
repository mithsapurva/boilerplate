
namespace Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an interface for IRepository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey>
       where TEntity : IBaseEntity<TKey>, new()
    {
        Task<TEntity> Get(TKey id);

        Task<IEnumerable<TEntity>> GetAll();

        Task Insert(TEntity entity);

        Task Insert(IEnumerable<TEntity> entities);

        Task<bool> Update(TEntity entity);

        Task Update(IEnumerable<TEntity> entities);
        Task Delete(TEntity entity);

        Task DeleteAll();

        Task Delete(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> Select(SelectOptions entities);

        Task<long> Count(object searchParameters);

        Task<long> Execute(string query, object parameters);
        Task<long> CountWithReadPast(object searchParameters);


    }
}
