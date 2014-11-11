using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace OXStack.Data
{
    /// <summary>
    /// Caching class
    /// </summary>
    public class CachingHelper
    {
        /// <summary>
        /// Minutes to expire.
        /// </summary>
        private int _iExpMins = 60;
        private ObjectCache _oc = null;
        /// <summary>
        /// MemoryCache property.
        /// </summary>
        private ObjectCache OCache { get { return _oc ?? (_oc = MemoryCache.Default); } }

        /// <summary>
        /// Get/set cache elements.. Use FlushParams to generate an index from cacheParams
        /// </summary>
        /// <param name="sIndex"></param>
        /// <returns></returns>
        public object this[string sIndex]
        {
            get { return GetCache(sIndex); }
            set { SetCache(sIndex, value); }
        }
        /// <summary>
        /// Get/set cache elements
        /// </summary>
        /// <param name="saIndex"></param>
        /// <returns></returns>
        public object this[string[] saIndex]
        {
            get { return GetCache(saIndex); }
            set { SetCache(saIndex, value); }
        }

        /// <summary>
        /// Tap into the cache, set amount of minutes to expire.
        /// </summary>
        /// <param name="iExpMins"></param>
        public CachingHelper(int iExpMins)
        {
            _iExpMins = iExpMins;
        }

        /// <summary>
        /// Generate key
        /// </summary>
        /// <param name="saKeys"></param>
        /// <returns></returns>
        public string FlushParams(string[] saKeys)
        {
            if (saKeys.Length == 0) return string.Empty;
            return string.Join("$$", saKeys);
        }

        /// <summary>
        /// Check if cache contains a certain element.
        /// </summary>
        /// <param name="saCacheParams"></param>
        /// <returns></returns>
        public bool Contains(string[] saCacheParams)
        {
            //
            return Contains(FlushParams(saCacheParams));
        }

        /// <summary>
        /// Check if cache contains a certain element. Use FlushParams to generate key from cacheParams
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public bool Contains(string sKey)
        {
            if (string.IsNullOrEmpty(sKey)) return false;

            return OCache.Contains(sKey);
        }


        /// <summary>
        /// Get element from cache
        /// </summary>
        /// <param name="saCacheParams"></param>
        /// <returns></returns>
        public object GetCache(string[] saCacheParams)
        {
            //
            return GetCache(FlushParams(saCacheParams));
        }

        private object GetCache(string sKey)
        {
            if (string.IsNullOrEmpty(sKey)) return null;

            return OCache.Get(sKey);
        }

        /// <summary>
        /// Write element to cache
        /// </summary>
        /// <param name="saCacheParams"></param>
        /// <param name="o"></param>
        public void SetCache(string[] saCacheParams, object o)
        {
            SetCache(FlushParams(saCacheParams), o);
        }

        private void SetCache(string sKey, object o)
        {
            if (string.IsNullOrEmpty(sKey)) return;

            // update
            OCache.Set(sKey, o, DateTime.Now.AddMinutes(_iExpMins));
        }

        // clear
        public void Clear()
        {
            List<string> lsKeys = new List<string>(CacheKeys());
            foreach (string sKey in lsKeys)
                OCache.Remove(sKey);
        }

        private IEnumerable<string> CacheKeys()
        {
            foreach (KeyValuePair<string, object> pair in OCache)
                yield return pair.Key;
        }


    }

}
