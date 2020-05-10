
namespace Data
{
    /// <summary>
    /// Defines interface for ISelectEntityWithChildStrategyFactory
    /// </summary>
    public interface ISelectEntityWithChildStrategyFactory
    {
        ISelectEntityWithChildStrategy<TEntity, TKey> GetStrategy<TEntity, TKey>()
            where TEntity : IBaseEntity<TKey>, new();
    }
}
