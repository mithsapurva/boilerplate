
namespace Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines interface for ISelectEntityWithChildStrategy
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ISelectEntityWithChildStrategy<TEntity, TKey>
        where TEntity : IBaseEntity<TKey>, new()
    {
        Task<IEnumerable<TEntity>> SelectEntityWithJoinedChild(
            IDbConnection connection,
            string query,
            object searchParameters
            );

        string GenerateSelectWithJoin(
            ISqlQueryPartsGenerator<TEntity, TKey> sqlPartsGenerator,
            object searchParameters,
            string forUpdatePart
            );
    }
}
