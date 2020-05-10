

namespace Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the interface for ICache
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Adds or replaces an object in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Add(string key, object value);

        /// <summary>
        /// Adds or replaces a dictionary object as hash in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The dictionary value.</param>
        /// <param name="hashfield">The hashfield that needs to be updated</param>
        void AddDictionaryToHash(string key, IDictionary<string, string> value, string hashField);

        /// <summary>
        /// Adds or replaces a dictionary object as hash in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The dictionary value.</param>
        void AddDictionaryToHash(string key, IDictionary<string, string> value);

        /// <summary>
        /// Adds or replaces an object as hash in the cache.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="keyValuePair">The key value pair</param>
        void AddKeyValuePairToHash(string key, KeyValuePair<string, string> keyValuePair);

        /// <summary>
        /// Adds value to Redis set
        /// </summary>
        /// <param name="redisKey">redis key parameter</param>
        /// <param name="value">value to be added in set</param>
        /// <returns>true is value is added</returns>
        bool AddToSet(string redisKey, string value);
        /// <summary>
        /// Gets a dictionary object from hash in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        IDictionary<string, string> GetDictionaryFromHash(string key);

        /// <summary>
        /// Gets the value associated with field in the hash stored at given key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="hashField">The field for which value needs to be fetched</param>
        /// <returns>The value associated with the field in the hash</returns>
        string GetValueFromHash(string key, string hashField);

        /// <summary>
        /// Gets an object from the cache using the specified key.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>object from the cache </returns>
        T GetObjectUsingKey<T>(string key);

        /// <summary>
        /// Gets an object from the cache using the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>object from the cache.</returns>
        object GetObject(string key);

        /// <summary>
        /// Gets the data from redis set
        /// </summary>
        /// <param name="redisKey">Redis key parameter to get data</param>
        /// <returns>List of values</returns>
        IList<string> GetDataFromSet(string redisKey);

        /// <summary>
        /// Gets the data from redis set
        /// </summary>
        /// <param name="redisKey">Redis key parameter to get data</param>
        /// <returns>List of values</returns>
        IList<string> GetDataFromSet(List<string> redisKeys);

        /// <summary>
        /// Removes the object from the cache using the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        /// <summary>
        /// Removes data from Set
        /// </summary>
        /// <param name="redisKey">redis key to be removed</param>
        /// <param name="value">value to be removed</param>
        void RemoveFromSet(string redisKey, string value);

        /// <summary>
        /// Removes all objects from the cache.
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Add data to sorted set
        /// </summary>
        /// <param name="redisKey">redis key to be removed</param>
        /// <param name="value">value to be removed</param>
        /// <param name="score">score of the value added</param>
        bool SortedSetAdd(string redisKey, string value, double score);

        /// <summary>
        /// Removes data from sorted Set that lies between a given range
        /// </summary>
        /// <param name="redisKey">redis key to be removed</param>
        /// <param name="startValue">Start value</param>
        /// <param name="endValue">Stop value</param>
        void SortedSetRemoveRangeByScore(string redisKey, double startValue, double endValue);
    }
}
