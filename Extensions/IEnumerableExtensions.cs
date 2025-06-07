using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Extensions
{
    public static class IEnumerableExtensions
    {
        public static Task<decimal> SumAsync<T>(this IEnumerable<T> source, Func<T, decimal> selector)
        {
            return Task.FromResult(source.Sum(selector));
        }

        public static Task<int> SumAsync<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            return Task.FromResult(source.Sum(selector));
        }

        public static Task<double> SumAsync<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            return Task.FromResult(source.Sum(selector));
        }

        public static Task<long> SumAsync<T>(this IEnumerable<T> source, Func<T, long> selector)
        {
            return Task.FromResult(source.Sum(selector));
        }
    }
} 