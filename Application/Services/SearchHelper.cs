using System.Linq.Expressions;
using System.Reflection;

namespace Api.Application.Services
{
    public static class SearchHelper
    {
        public static Expression<Func<TEntity, bool>> BuildGlobalSearchPredicate<TEntity>(List<string> searchTerms, string[]? excludedProperties = null)
        {
            Expression<Func<TEntity, bool>> predicate = x => false;
            var searchableProperties = GetSearchableProperties<TEntity>(excludedProperties);

            foreach (var term in searchTerms)
            {
                if (string.IsNullOrWhiteSpace(term)) continue;
                var termPredicate = BuildTermPredicate<TEntity>(term, searchableProperties);
                predicate = CombineOr(predicate, termPredicate);
            }

            return predicate;
        }

        private static PropertyInfo[] GetSearchableProperties<TEntity>(string[]? excludedProperties)
        {
            return [.. typeof(TEntity).GetProperties().Where(p => excludedProperties == null || !excludedProperties.Contains(p.Name))];
        }

        public static Expression<Func<TEntity, bool>> BuildTermPredicate<TEntity>(string term, PropertyInfo[] searchableProperties)
        {
            var param = Expression.Parameter(typeof(TEntity), "p");
            Expression finalExpression = Expression.Constant(false);

            foreach (var prop in searchableProperties)
            {
                try
                {
                    Type propType = prop.PropertyType;
                    Type targetType = Nullable.GetUnderlyingType(propType) ?? propType;

                    // Skip unsupported types
                    if (targetType != typeof(string) && !IsNumericType(targetType))
                        continue;

                    Expression propertyAccess = Expression.Property(param, prop);
                    Expression condition;

                    if (targetType == typeof(string))
                    {
                        condition = BuildStringCondition(propertyAccess, term);
                    }
                    else
                    {
                        // Handle nullable conversion for numeric types
                        condition = BuildNumericCondition(propertyAccess, term, propType, targetType);
                    }

                    finalExpression = Expression.OrElse(finalExpression, condition);
                }
                catch
                {
                    // Skip invalid conversions
                    continue;
                }
            }

            return Expression.Lambda<Func<TEntity, bool>>(finalExpression, param);
        }

        private static Expression BuildStringCondition(Expression property, string term)
        {
            // Handle null-check for string properties
            var notNull = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));

            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])
                ?? throw new InvalidOperationException("Contains method not found.");

            var containsCall = Expression.Call(property, containsMethod, Expression.Constant(term));
            return Expression.AndAlso(notNull, containsCall);
        }

        private static Expression BuildNumericCondition(Expression property, string term, Type propType, Type targetType)
        {
            // Skip if conversion fails or converted value is null
            if (!TryConvert(term, targetType, out object? convertedValue) || convertedValue is null)
                return Expression.Constant(false);

            // Handle nullable conversion
            Expression constant = propType.IsGenericType &&
                                  propType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Expression.Convert(Expression.Constant(convertedValue), propType)
                : Expression.Constant(convertedValue);

            return Expression.Equal(property, constant);
        }

        private static bool IsNumericType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return Type.GetTypeCode(underlyingType) switch
            {
                TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16
                or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16
                or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal
                or TypeCode.Double or TypeCode.Single => true,
                _ => false
            };
        }

        private static bool TryConvert(string value, Type targetType, out object? result)
        {
            result = null;
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                result = Convert.ChangeType(value, underlyingType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Expression<Func<T, bool>> CombineOr<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T));
            var combined = Expression.OrElse(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param));
            return Expression.Lambda<Func<T, bool>>(combined, param);
        }
    }
}