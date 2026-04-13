using System.Collections.Generic;
using System.Linq;
namespace Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> source)
        {
            int index = 0;
            foreach (T value in source)
            {
                yield return (index, value);
                index++;
            }
        }
        
        public static IEnumerable<(T current, T next)> WithNext<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    yield break; 

                T current = enumerator.Current;
                T next = current;

                while (enumerator.MoveNext())
                {
                    next = enumerator.Current;
                    yield return (current, next); 
                    current = next;
                }
                
                yield return (current, source.First());
            }
        }
    }
}