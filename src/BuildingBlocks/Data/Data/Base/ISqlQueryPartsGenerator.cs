
namespace Data
{
    /// <summary>
    /// Defines interface for ISqlQueryPartsGenerator
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ISqlQueryPartsGenerator<TEntity, TKey>
        where TEntity : IBaseEntity<TKey>, new()
    {
        string GetTableName<TEntityTable, TEntityPkType>()
                where TEntityTable : IBaseEntity<TEntityPkType>, new();

        string GenerateSelect(object fieldsFilter, string topCount);
        string GenerateSelectWithJoin<TChild, TChilkPkType>(object searchParameters, string childForeignKey, string forUpdatePart)
            where TChild : IBaseEntity<TChilkPkType>, new();


        string GenerateSelect(object fieldsFilter, string additionalParameters, string joinPart);
        string GenerateSelect(string topCount);
        string GenerateOrderBy(object ColumnsToOrder, bool ascending);

        string GenerateWhere(object filterColumns);
        string GenerateCount(object filterColumns);
        string GenerateCountWithReadPast(object filterColumns);
        string GenerateOrderBy(string columnName, bool ascending = true);
        string GenerateSelectTop(int rowSelectLimit);
        string GenerateLockForUpdate();
        string GenerateWithIndex(string index);

    }
}
