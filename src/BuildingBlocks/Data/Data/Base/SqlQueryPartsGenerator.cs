

namespace Data
{
    using Castle.Core.Internal;
    using System;
    using System.Collections;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Defines the class for SqlQueryPartsGenerator
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class SqlQueryPartsGenerator<TEntity, TKey> : ISqlQueryPartsGenerator<TEntity, TKey>
       where TEntity : IBaseEntity<TKey>, new()
    {
        private const string OperatorIn = "IN";

        private const string OperatorEqual = "=";
        private const string OperatorNotIN = "NOT IN";
        private const string OperatorNotEqual = "<>";
        private const string OperatorGreater = ">";
        private const string OperatorGreaterEqual = ">=";
        private const string OperatorLess = "<";
        private const string OperatorLessEqual = "<=";

        private const string ExcludeWord = "EXCLUDE";
        private const string GreaterWord = "GREATER";
        private const string GreaterEqualWord = "GREATEREQUAL";
        private const string LessWord = "LESS";
        private const string LessEqualWord = "LESSEQUAL";

        private readonly string[] _propertiesName;
        private readonly string _tableName;

        public SqlQueryPartsGenerator()
        {

            Type type = typeof(TEntity);
            PropertyInfo[] properties = type.GetProperties();
            _propertiesName = properties.Where(a => !IsComplexType(a))
                .Select(a => a.GetAttribute<ColumnAttribute>()?.Name ?? a.Name)
                .ToArray();
            _tableName = GetTableName<TEntity, TKey>();
        }

        private static bool IsComplexType(PropertyInfo propertyInfo)
        {
            bool result = (propertyInfo.PropertyType.GetTypeInfo().IsClass && propertyInfo.PropertyType != typeof(string))
                || propertyInfo.PropertyType.GetTypeInfo().IsInterface
                || propertyInfo.GetCustomAttribute<NotMappedAttribute>() != null;

            return result;
        }
        public string GenerateCount(object filterColumns)
        {
            return $"SELECT COUNT(*) {GenerateFromPart()} {GenerateWhere(filterColumns)}";
        }

        public string GenerateCountWithReadPast(object filterColumns)
        {
            return $"SELECT COUNT(*) {GenerateFromPart()} WITH (READPAST) {GenerateWhere(filterColumns)}";
        }

        public string GenerateLockForUpdate()
        {
            if (System.Transactions.Transaction.Current == null)
            {
                throw new ArgumentException("No open transaction");
            }
            return "WITH (UPDLOCK, ROWLOCK, READPAST, READCOMMITTEDLOCK)";
        }

        public string GenerateOrderBy(object columnsToOrder, bool ascending)
        {
            string[] orderColumns = columnsToOrder.GetType().GetProperties().Select(column => $"targetEntity.[{column.Name}]").ToArray();
            string orderType = ascending ? "asc" : "desc";
            return $"ORDER BY {string.Join(",", orderColumns)}{orderType}";
        }

        public string GenerateOrderBy(string columnName, bool ascending = true)
        {
            var sortPart = ascending ? string.Empty : "DESC";
            return $" ORDER BY {columnName} {sortPart}";
        }

        public string GenerateSelect(object fieldsFilter, string topCount)
        {
            string selectPart = GenerateSelect(topCount);
            string wherePart = GenerateWhere(fieldsFilter);
            return $"{selectPart} {wherePart}";
        }

        public string GenerateSelect(object fieldsFilter, string additionalSelectParameters, string joinPart)
        {
            var select = new StringBuilder("SELECT ");
            string separator = $", {Environment.NewLine}";
            string columnsToSelect = string.Join(separator, _propertiesName.Select(name => $"targetEntity.[{name}]"));
            select.AppendLine(columnsToSelect);
            if (!string.IsNullOrWhiteSpace(additionalSelectParameters))
            {
                select.AppendLine($", {additionalSelectParameters}");
            }
            string fromPart = GenerateFromPart();
            select.Append(fromPart);

            string selectPart = select.ToString();
            string wherePart = GenerateWhere(fieldsFilter);
            return $" {selectPart} {joinPart} {wherePart}";
        }

        public string GenerateSelect(string topCount)
        {
            var sb = new StringBuilder($"SELECT {topCount}");
            string separator = $", {Environment.NewLine}";
            string selectPart = string.Join(separator, _propertiesName.Select(name => $"targetEntity.[{name}]"));
            sb.AppendLine(selectPart);
            string fromPart = $"FROM {_tableName} targetEntity";
            sb.Append(fromPart);
            return sb.ToString();
        }

        public string GenerateSelectTop(int rowSelectLimit)
        {
            return $" TOP ({rowSelectLimit})";
        }

        public string GenerateSelectWithJoin<TChild, TChilkPkType>(object searchParameters, string childForeignKey, string forUpdatePart) where TChild : IBaseEntity<TChilkPkType>, new()
        {
            return GenerateSelect(
                searchParameters,
                "child.*",
                $"LEFT JOIN {GetTableName<TChild, TChilkPkType>()} child ON targetEntity.[{nameof(IBaseEntity<TChilkPkType>.Id)}] = child.[{childForeignKey}]"
                );
        }

        public string GenerateWhere(object filterColumns)
        {
            var filterPropertyInfoArray = filterColumns.GetType().GetProperties().ToArray();
            string[] filter = filterPropertyInfoArray.Select(info => GenerateSingleWhereCondition(info, filterColumns)).ToArray();
            string wherePart = string.Join(" AND ", filter);
            return $" WHERE {wherePart} ";
        }

        private string GenerateSingleWhereCondition(PropertyInfo info, object filterColumns)
        {
            string whereOperator;
            string columnName;
            var value = info.GetValue(filterColumns);
            if (info.Name.Contains(ExcludeWord))
            {
                columnName = info.Name.Replace(ExcludeWord, string.Empty);
                if (value == null)
                {
                    return $"targetEntity.[{columnName}] IS NOT NULL";

                }
                whereOperator = IsArrayProperty(info)
                     ? OperatorNotIN
                     : OperatorNotEqual;
            }
            else
            {
                columnName = info.Name;
                if (value == null)
                {
                    return $"targetEntity.[{columnName}] IS NULL";

                }
                var whereInfo = GenerateComparison(info);
                columnName = whereInfo.Item1;
                whereOperator = whereInfo.Item2;

            }
            return $"targetEntity.[{columnName}] {whereOperator} @{info.Name}";
        }

        public string GenerateWithIndex(string index)
        {
            return $"WITH (INDEX({index}))";
        }

        public string GetTableName<TEntityTable, TEntityPkType>() where TEntityTable : IBaseEntity<TEntityPkType>, new()
        {
            Type type = typeof(TEntityTable);
            return type.GetAttribute<TableAttribute>()?.Name ?? type.Name; //castle.core
        }

        private static bool IsArrayProperty(PropertyInfo propertyInfo)
        {
            return typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType)
                    && !(typeof(string) == propertyInfo.PropertyType);
        }
        private string GenerateFromPart()
        {
            return $"FROM {_tableName} targetEntity";
        }

        private static Tuple<string, string> GenerateComparison(PropertyInfo propertyInfo)
        {
            if (IsArrayProperty(propertyInfo))
            {
                return Tuple.Create(propertyInfo.Name, OperatorIn);
            }
            Tuple<string, string> compare = null;
            if (propertyInfo.Name.Contains(GreaterEqualWord))
            {
                compare = Tuple.Create(propertyInfo.Name.Replace(GreaterEqualWord, String.Empty), OperatorGreaterEqual);
            }
            else if (propertyInfo.Name.Contains(GreaterWord))
            {
                compare = Tuple.Create(propertyInfo.Name.Replace(GreaterWord, String.Empty), OperatorGreater);
            }
            else if (propertyInfo.Name.Contains(LessEqualWord))
            {
                compare = Tuple.Create(propertyInfo.Name.Replace(LessEqualWord, String.Empty), OperatorLessEqual);
            }
            else if (propertyInfo.Name.Contains(LessWord))
            {
                compare = Tuple.Create(propertyInfo.Name.Replace(LessWord, String.Empty), OperatorLess);
            }
            else
            {
                compare = Tuple.Create(propertyInfo.Name, OperatorEqual);
            }
            return compare;
        }
    }
}
