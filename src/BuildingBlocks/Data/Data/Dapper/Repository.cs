

namespace Data
{
    using Castle.Core.Internal;
    using Dapper;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the class for Repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
          where TEntity : class, IBaseEntity<TKey>, new()
    {
        private const int SqlCopyBatchSize = 5000;

        private readonly ISelectEntityWithChildStrategyFactory _selectEntityWithChildStrategyFactory;

        private readonly string _tableName;

        protected Repository(IConnectionFactory connectionFactory,
            ISqlQueryPartsGenerator<TEntity, TKey> partsGenerator,
            ISelectEntityWithChildStrategyFactory selectEntityWithChildStrategyFactory)
        {
            _selectEntityWithChildStrategyFactory = selectEntityWithChildStrategyFactory;
            ConnectionFactory = connectionFactory;
            PartsGenerator = partsGenerator;
            _tableName = PartsGenerator.GetTableName<TEntity, TKey>();
        }

        protected abstract string UpdateSql { get; }

        protected IConnectionFactory ConnectionFactory { get; }
        protected ISqlQueryPartsGenerator<TEntity, TKey> PartsGenerator { get; }

        public virtual async Task<TEntity> Get(TKey id)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                return (await wrapper.Connection.QueryAsync<TEntity>(
                    $"SELECT * FROM [{_tableName}] WHERE [Id] = @Id",
                    new { Id = id })).SingleOrDefault();
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                return await wrapper.Connection.QueryAsync<TEntity>($"SELECT * FROM [{_tableName}]") ??
                    new List<TEntity>();
            }
        }

        public virtual async Task Insert(TEntity entity)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                await ExecuteInsert(wrapper, entity);
            }
        }

        public virtual async Task Insert(IEnumerable<TEntity> entities)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                foreach (var entity in entities)
                {
                    await ExecuteInsert(wrapper, entity);
                }
            }
        }

        public virtual async Task<bool> Update(TEntity entity)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                return await ExecuteUpdate(wrapper, entity);
            }
        }

        public virtual async Task Update(IEnumerable<TEntity> entities)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                foreach (var entity in entities)
                {
                    await ExecuteUpdate(wrapper, entity);
                }
            }
        }

        public virtual async Task Delete(TEntity entity)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                await ExecuteDelete(wrapper, entity);
            }
        }

        public virtual async Task Delete(IEnumerable<TEntity> entities)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                foreach (var entity in entities)
                {
                    await ExecuteDelete(wrapper, entity);
                }
            }
        }

        public async Task DeleteAll()
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                await wrapper.Connection.ExecuteAsync($"DELETE FROM [{_tableName}]");
            }
        }

        public virtual async Task<IEnumerable<TEntity>> Select(SelectOptions options)
        {
            var orderPart = options.OrderByColumns != null && options.OrderAscending.HasValue
                ? PartsGenerator.GenerateOrderBy(options.OrderByColumns, options.OrderAscending.Value)
                : string.Empty;
            var topPart = options.TopCount.HasValue ? PartsGenerator.GenerateSelectTop(options.TopCount.Value) : string.Empty;
            var forUpdatePart = options.IsForUpdate ? PartsGenerator.GenerateLockForUpdate() : string.Empty;
            var additionalParts = $"{orderPart}";

            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                ISelectEntityWithChildStrategy<TEntity, TKey> strategy = _selectEntityWithChildStrategyFactory.GetStrategy<TEntity, TKey>();
                if (options.JoinChild && strategy != null)
                {
                    return await strategy.SelectEntityWithJoinedChild(
                        wrapper.Connection,
                        $"{strategy.GenerateSelectWithJoin(PartsGenerator, options.SearchParameters, forUpdatePart)} {additionalParts}",
                        options.SearchParameters
                        );
                }
                var query =
                    $"{PartsGenerator.GenerateSelect(topPart)} {forUpdatePart} {PartsGenerator.GenerateWhere(options.SearchParameters)}{orderPart}";
                return (await wrapper.Connection.QueryAsync<TEntity>(query, options.SearchParameters))
                    .AsList() ??
                    new List<TEntity>();
            }
        }

        public virtual async Task<long> Count(object searchParameters)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                return (await wrapper.Connection.QueryAsync<long>(PartsGenerator.GenerateCount(searchParameters), searchParameters))
                    .Single();
            }
        }

        public async Task<long> Execute(string query, object parameters)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                return await wrapper.Connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task<long> CountWithReadPast(object searchParameters)
        {
            using (IConnectionWrapper wrapper = ConnectionFactory.GetConnection())
            {
                return (await wrapper.Connection.QueryAsync<long>(PartsGenerator.GenerateCountWithReadPast(searchParameters), searchParameters))
                    .Single();
            }
        }

        protected abstract Task ExecuteInsert(IConnectionWrapper wrapper, TEntity entity);

        protected virtual async Task ExecuteDelete(IConnectionWrapper wrapper, TEntity entity)
        {
            await wrapper.Connection.ExecuteAsync(
                $"DELETE FROM [{_tableName}] WHERE [Id] = @Id", new { Id = entity.Id });
        }

        private async Task<bool> ExecuteUpdate(IConnectionWrapper wrapper, TEntity entity)
        {
            return (await wrapper.Connection.ExecuteAsync(UpdateSql, entity)) > 0;
        }

        private void ConfigureColumnMapping()
        {
            var type = typeof(TEntity);
            var mappedProperties = type.GetProperties()
                .Where(p => p.GetAttribute<ColumnAttribute>() != null)?
                .Select(p => p)
                .ToDictionary(p => p.GetAttribute<ColumnAttribute>().Name);
            if (mappedProperties != null)
            {
                var map = new CustomPropertyTypeMap(
                    type,
                    (t, column) =>
                    {
                        if (mappedProperties.TryGetValue(column, out var info))
                        {
                            return info;
                        }
                        else
                        {
                            return t.GetProperty(column);
                        }
                    });
                SqlMapper.SetTypeMap(type, map);
            }
        }
    }
}
