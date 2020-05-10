

namespace Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface for IMemoryCache
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMemoryCache<TKey, TValue>
    {
        IEnumerable<TValue> Values { get; }

        IEnumerable<TKey> Keys { get; }

        void Add(TKey key, TValue value);

        bool TryGet(TKey key, out TValue value);

        void Remove(TKey key);
    }
}
