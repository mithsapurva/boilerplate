

namespace Common
{
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Defines RedisCache class
    /// </summary>
    public class RedisCache : ICache, IDisposable
    {
        /// <summary>
        /// Defines readonly field for REDIS_URL
        /// </summary>
        private static readonly string REDIS_URL = "http://redis:6379";

        /// <summary>
        /// The _DB
        /// </summary>
        private static IDatabase _db;

        /// <summary>
        /// Prevents a default instance of the <see cref="RedisCache"/> class from being created.
        /// </summary>        
        public RedisCache()
        {
            Connect();
            SetupTypes();
        }

        //initial setup code for types if any     
        private static void SetupTypes()
        {
            // to do
        }

        #region Redis Connection Methods

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        private static void Connect()
        {
            if (_db == null)
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,    // will not serialize an object if it is a child object of itself
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,  // is useful if objects are nested but not indefinitely
                    //PreserveReferencesHandling = PreserveReferencesHandling.Objects, // serialize an object that is nested indefinitely
                    TypeNameHandling = TypeNameHandling.All
                };

                ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(REDIS_URL.ToString());
                _db = _redis.GetDatabase();
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public static bool Disconnect()
        {
            _db = null;
            return true;
        }
        #endregion

        #region ICache Methods

        /// <summary>
        /// Adds or replaces an object in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            _db.StringSet(key, json);
        }

        /// <summary>
        /// Adds or replaces a dictionary object as hash in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The dictionary value.</param>
        /// <param name="hashfield">The hashfield that needs to be updated</param>
        public void AddDictionaryToHash(string key, IDictionary<string, string> value, string hashField)
        {
            if (value != null && value.Count > 0)
            {
                if (hashField != null)
                {
                    _db.HashSet(key, hashField, value[hashField]);
                }
                else
                {
                    HashEntry[] fields = value.Select(pair => new HashEntry(pair.Key, pair.Value)).ToArray();
                    _db.HashSet(key, fields);
                }
            }
        }

        /// <summary>
        /// Adds or replaces an object as hash in the cache.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="keyValuePair">The key value pair</param>
        public void AddKeyValuePairToHash(string key, KeyValuePair<string, string> keyValuePair)
        {
            _db.HashSet(key, keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Adds or replaces a dictionary object as hash in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The dictionary value.</param>        
        public void AddDictionaryToHash(string key, IDictionary<string, string> value)
        {
            AddDictionaryToHash(key, value, null);
        }

        /// <summary>
        /// Gets a dictionary object from hash in the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        public IDictionary<string, string> GetDictionaryFromHash(string key)
        {
            HashEntry[] hashEntries = _db.HashGetAll(key);
            Dictionary<string, string> result = null;

            if (hashEntries != null && hashEntries.Length > 0)
            {
                result = hashEntries.OrderBy(entry => entry.Value)
                    .ToDictionary(entry => Convert.ToString(entry.Name, CultureInfo.InvariantCulture), entry => Convert.ToString(entry.Value, CultureInfo.InvariantCulture));
            }
            return result;
        }

        /// <summary>
        /// Gets a dictionary object of selected fields from hash in the cache.
        /// </summary>
        /// <param name="key">The hash key</param>
        /// <param name="hashFields">List of hash-fields that needs to be populated</param>
        /// <returns></returns>
        public Dictionary<string, string> GetDictionaryFromHash(string key, string[] hashFields)
        {
            Dictionary<string, string> result = null;

            if (hashFields != null)
            {
                var redisValues = Array.ConvertAll(hashFields, item => (RedisValue)item);

                var hashsetData = _db.HashGet(key, redisValues).ToStringArray();

                result = new Dictionary<string, string>(hashsetData.Length, StringComparer.Ordinal);

                for (int i = 0; i < hashsetData.Length; i++)
                {
                    // check against null because we get a null value if the key doesn't exist in Redis
                    if (hashsetData[i] != null)
                    {
                        result.Add(hashFields[i], hashsetData[i]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the value associated with field in the hash stored at given key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="hashField">The field for which value needs to be fetched</param>
        /// <returns>The value associated with the field in the hash</returns>
        public string GetValueFromHash(string key, string hashField)
        {
            return _db.HashGet(key, hashField);
        }

        /// <summary>
        /// Gets an object from the cache using the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// object from the cache.
        /// </returns>
        public object GetObject(string key)
        {
            string json = _db.StringGet(key);
            object value = null;
            if (!string.IsNullOrEmpty(json))
            {
                value = JsonConvert.DeserializeObject(json);
            }
            return value;
        }

        /// <summary>
        /// Gets an object from the cache using the specified key.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>
        /// object from the cache
        /// </returns>
        public T GetObjectUsingKey<T>(string key)
        {
            string json = _db.StringGet(key);
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            T value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }

        /// <summary>
        /// Removes the object from the cache using the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }

        /// <summary>
        /// Removes all objects from the cache.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds value to Redis set
        /// </summary>
        /// <param name="redisKey">redis key parameter</param>
        /// <param name="value">value to be added in set</param>
        /// <returns>true is value is added</returns>
        public bool AddToSet(string redisKey, string value)
        {
            return _db.SetAdd(redisKey, value);
        }

        /// <summary>
        /// Gets the data from redis set
        /// </summary>
        /// <param name="redisKey">Redis key parameter to get data</param>
        /// <returns>List of values</returns>
        public IList<string> GetDataFromSet(string redisKey)
        {
            var data = _db.SetMembers(redisKey).Select(m => Convert.ToString(m, CultureInfo.InvariantCulture)).ToList();
            return data;
        }

        /// <summary>
        /// Gets the data from redis set
        /// </summary>
        /// <param name="redisKeys">Redis keys parameter to get data</param>
        /// <returns>List of values</returns>
        public IList<string> GetDataFromSet(List<string> redisKeys)
        {
            IList<string> result = new List<string>();

            if (redisKeys != null && redisKeys.Any())
            {
                var keys = redisKeys.Select(key => (RedisKey)key).ToArray();
                result = _db.SetCombine(SetOperation.Union, keys).ToStringArray().ToList();
            }
            return result;
        }

        /// <summary>
        /// Removes data from Set
        /// </summary>
        /// <param name="redisKey">redis key to be removed</param>
        /// <param name="value">value to be removed</param>
        public void RemoveFromSet(string redisKey, string value)
        {
            _db.SetRemove(redisKey, value);
        }

        /// <summary>
        /// Removes from hash.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hashField">The hash field.</param>
        /// <returns>operation successful or not</returns>
        public bool RemoveFromHash(string key, string hashField)
        {
            return _db.HashDelete(key, hashField);
        }

        /// <summary>
        /// Removes from hash.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hashFields">The hash fields.</param>
        /// <returns>operation successful or not</returns>
        public long RemoveFromHash(string key, string[] hashFields)
        {
            long result = 0;
            if (hashFields != null)
            {
                var values = Array.ConvertAll(hashFields, item => (RedisValue)(item));
                result = _db.HashDelete(key, values);
            }
            return result;
        }


        /// <summary>
        /// Add data to sorted set
        /// </summary>
        /// <param name="redisKey">redis key to be removed</param>
        /// <param name="value">value to be removed</param>
        /// <param name="score">score of the value added</param>
        public bool SortedSetAdd(string redisKey, string value, double score)
        {
            return _db.SortedSetAdd(redisKey, value, score);
        }

        /// <summary>
        /// Removes data from sorted Set that lies between a given range
        /// </summary>
        /// <param name="redisKey">redis key to be removed</param>
        /// <param name="startValue">Start value</param>
        /// <param name="endValue">Stop value</param>
        public void SortedSetRemoveRangeByScore(string redisKey, double startValue, double endValue)
        {
            _db.SortedSetRemoveRangeByScore(redisKey, startValue, endValue);
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Destructor added to call Dispose with false value;
        /// </summary>
        ~RedisCache()
        {
            Dispose(false);
        }
    }
}
