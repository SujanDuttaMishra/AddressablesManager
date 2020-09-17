using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AddressableManager
{
    public static class LinqExtension
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) Debug.LogException(new NullReferenceException());
            if (action == null) Debug.LogException(new NullReferenceException());
            foreach (var element in source) { action(element); }
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var index = 0;
            foreach (var element in source) action(element, index++);
        }
        public static void ForEachWithNullCheck<T>(this List<T> list, Action<T> action) => list?.ForEach(action);

        public static IEnumerable<TSource> Assert<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Assert(source, predicate, null);
        }

        public static IEnumerable<TSource> Assert<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, Func<TSource, Exception> errorSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return _();
            IEnumerable<TSource> _()
            {
                foreach (var element in source)
                {
                    var success = predicate(element);
                    if (!success) throw errorSelector?.Invoke(element) ?? new InvalidOperationException("Sequence contains an invalid item.");
                    yield return element;
                }
            }
        }

        public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<T> source,
            Func<T, (bool, TResult)> chooser)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (chooser == null) throw new ArgumentNullException(nameof(chooser));

            return _(); IEnumerable<TResult> _()
            {
                foreach (var item in source)
                {
                    var (some, value) = chooser(item);
                    if (some)
                        yield return value;
                }
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return _(); IEnumerable<TSource> _()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) =>
            source.ToDictionary(null);

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.ToDictionary(e => e.Key, e => e.Value, comparer);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey Key, TValue Value)> source) => source.ToDictionary(null);

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey Key, TValue Value)> source, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.ToDictionary(e => e.Key, e => e.Value, comparer);
        }
    }
}

