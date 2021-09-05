using System;
using System.Collections.Generic;

namespace HFM.Log.Internal
{
    internal static class CollectionExtensions
    {
        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey,TValue}" /> if the key does not already exist.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns>true if the key did not already exist and the provided key and value were added; otherwise, false.</returns>
        internal static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }
            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of element to return.</typeparam>
        /// <param name="source">An <see cref="IList{T}"/> to return the last element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <returns>default(TResult) if the source sequence is empty; otherwise, the last element in the <see cref="IList{T}"/>.</returns>
        internal static TResult LastOrDefault<TSource, TResult>(this IList<TSource> source, Func<TResult, bool> predicate) where TResult : TSource
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            for (int i = source.Count - 1; i >= 0; i--)
            {
                var x = (TResult)source[i];
                if (predicate(x))
                {
                    return x;
                }
            }

            return default;
        }
    }
}
