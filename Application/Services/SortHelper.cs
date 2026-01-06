using System.Linq.Expressions;

namespace Api.Application.Services
{
    public static class SortHelper
    {
        public static IQueryable<TEntity> ApplySorting<TEntity, TSort>(
            IQueryable<TEntity> query,
            IList<TSort>? sortFields,
            Func<TSort, string> fieldSelector,
            Func<TSort, string> directionSelector)
        {
            if (sortFields == null || !sortFields.Any())
                return query;

            IOrderedQueryable<TEntity>? orderedQuery = null;
            bool isFirstSort = true;

            foreach (var sort in sortFields)
            {
                var fieldName = fieldSelector(sort);
                var direction = directionSelector(sort);

                // Handle special case for CategoryName
                if (fieldName.Equals("CategoryName", StringComparison.OrdinalIgnoreCase))
                {
                    fieldName = "Category.Name";
                }

                if (isFirstSort)
                {
                    orderedQuery = direction.Equals("desc", StringComparison.OrdinalIgnoreCase)
                        ? OrderByDescending(query, fieldName)
                        : OrderBy(query, fieldName);
                    isFirstSort = false;
                }
                else if (orderedQuery != null)
                {
                    orderedQuery = direction.Equals("desc", StringComparison.OrdinalIgnoreCase)
                        ? ThenByDescending(orderedQuery, fieldName)
                        : ThenBy(orderedQuery, fieldName);
                }
            }

            return orderedQuery ?? query;
        }

        private static IOrderedQueryable<T> OrderBy<T>(IQueryable<T> source, string propertyPath)
        {
            return ApplyOrder(source, propertyPath, "OrderBy")!;
        }

        private static IOrderedQueryable<T> OrderByDescending<T>(IQueryable<T> source, string propertyPath)
        {
            return ApplyOrder(source, propertyPath, "OrderByDescending")!;
        }

        private static IOrderedQueryable<T> ThenBy<T>(IOrderedQueryable<T> source, string propertyPath)
        {
            return ApplyOrder(source, propertyPath, "ThenBy")!;
        }

        private static IOrderedQueryable<T> ThenByDescending<T>(IOrderedQueryable<T> source, string propertyPath)
        {
            return ApplyOrder(source, propertyPath, "ThenByDescending")!;
        }

        private static IOrderedQueryable<T>? ApplyOrder<T>(IQueryable<T> source, string propertyPath, string methodName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression property = parameter;

            // Handle nested properties
            foreach (var member in propertyPath.Split('.'))
            {
                property = Expression.Property(property, member);
            }

            var lambda = Expression.Lambda(property, parameter);

            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .SingleOrDefault()
                ?? throw new InvalidOperationException($"Method {methodName} not found on Queryable.");

            var genericMethod = method.MakeGenericMethod(typeof(T), property.Type);

            var result = genericMethod.Invoke(null, [source, lambda]);
            return result as IOrderedQueryable<T>;
        }
    }
}