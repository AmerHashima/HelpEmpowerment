using System.Linq.Expressions;
using System.Reflection;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, List<FilterRequest> filters)
        {
            if (filters == null || !filters.Any())
                return query;

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.PropertyName))
                    continue;

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = GetProperty<T>(parameter, filter.PropertyName);

                if (property == null)
                    continue;

                var filterExpression = BuildFilterExpression(parameter, property, filter);
                if (filterExpression != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                    query = query.Where(lambda);
                }
            }

            return query;
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, List<SortRequest> sorts)
        {
            if (sorts == null || !sorts.Any())
                return query;

            IOrderedQueryable<T>? orderedQuery = null;

            for (int i = 0; i < sorts.Count; i++)
            {
                var sort = sorts[i];
                if (string.IsNullOrWhiteSpace(sort.SortBy))
                    continue;

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = GetProperty<T>(parameter, sort.SortBy);

                if (property == null)
                    continue;

                var lambda = Expression.Lambda(property, parameter);
                var methodName = i == 0
                    ? (sort.SortDirection.ToLower() == "desc" ? "OrderByDescending" : "OrderBy")
                    : (sort.SortDirection.ToLower() == "desc" ? "ThenByDescending" : "ThenBy");

                var orderByMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type);

                orderedQuery = (IOrderedQueryable<T>)orderByMethod.Invoke(null, new object[] { i == 0 ? query : orderedQuery!, lambda })!;
            }

            return orderedQuery ?? query;
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationRequest pagination)
        {
            if (pagination == null || pagination.GetAll)
                return query;

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
            var take = pagination.PageSize;

            return query.Skip(skip).Take(take);
        }

        private static Expression? GetProperty<T>(Expression parameter, string propertyPath)
        {
            try
            {
                Expression property = parameter;
                var properties = propertyPath.Split('.');

                foreach (var prop in properties)
                {
                    var propInfo = property.Type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                        return null;

                    property = Expression.Property(property, propInfo);
                }

                return property;
            }
            catch
            {
                return null;
            }
        }

        private static Expression? BuildFilterExpression(Expression parameter, Expression property, FilterRequest filter)
        {
            try
            {
                var value = ConvertValue(filter.Value, property.Type);
                var constant = Expression.Constant(value, property.Type);

                return filter.Operation switch
                {
                    FilterOperation.Equal => Expression.Equal(property, constant),
                    FilterOperation.NotEqual => Expression.NotEqual(property, constant),
                    FilterOperation.GreaterThan => Expression.GreaterThan(property, constant),
                    FilterOperation.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
                    FilterOperation.LessThan => Expression.LessThan(property, constant),
                    FilterOperation.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
                    FilterOperation.Contains => BuildStringMethod(property, constant, "Contains"),
                    FilterOperation.StartsWith => BuildStringMethod(property, constant, "StartsWith"),
                    FilterOperation.EndsWith => BuildStringMethod(property, constant, "EndsWith"),
                    FilterOperation.In => BuildInExpression(property, filter.Value),
                    FilterOperation.IsNull => Expression.Equal(property, Expression.Constant(null, property.Type)),
                    FilterOperation.IsNotNull => Expression.NotEqual(property, Expression.Constant(null, property.Type)),
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }


        private static Expression? BuildStringMethod(Expression property, Expression value, string methodName)
        {
            if (property.Type != typeof(string))
                return null;

            var method = typeof(string).GetMethod(methodName, new[] { typeof(string) });
            if (method == null)
                return null;

            return Expression.Call(property, method, value);
        }

        private static Expression? BuildInExpression(Expression property, object? value)
        {
            if (value == null)
                return null;

            var list = value as IEnumerable<object> ?? new[] { value };
            var convertedList = list.Select(v => ConvertValue(v, property.Type)).ToList();

            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(property.Type);

            var valueList = Expression.Constant(convertedList);
            return Expression.Call(containsMethod, valueList, property);
        }

        private static object? ConvertValue(object? value, Type targetType)
        {
            if (value == null)
                return null;

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType.IsEnum)
            {
                if (value is string strValue)
                    return Enum.Parse(underlyingType, strValue, true);
                return Enum.ToObject(underlyingType, value);
            }

            if (underlyingType == typeof(Guid))
            {
                if (value is string guidStr)
                    return Guid.Parse(guidStr);
                return value;
            }

            if (underlyingType == typeof(DateTime))
            {
                if (value is string dateStr)
                    return DateTime.Parse(dateStr);
                return value;
            }

            return Convert.ChangeType(value, underlyingType);
        }
    }
}