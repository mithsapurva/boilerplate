

namespace Common
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the abstract class for MemoryCache
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class MemoryCache<TKey, TValue> : IMemoryCache<TKey, TValue>
    {
        public IEnumerable<TValue> Values => Cache.Values;

        public IEnumerable<TKey> Keys => Cache.Keys;

        private ConcurrentDictionary<TKey, TValue> Cache { get; } = new ConcurrentDictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            Cache[key] = value;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return Cache.TryGetValue(key, out value);
        }
        public void Remove(TKey key)
        {
            Cache.TryRemove(key, out var _);
        }
    }
}
