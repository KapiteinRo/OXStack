using System;
using System.Collections.Generic;
using System.Data;
using OXStack.Data;
using OXStack.Config;
using OXStack.Helpers;

namespace OXStack.RT
{
    /// <summary>
    /// Basis set
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRT<T> : List<T>, IBaseBare
    {
#region FROM_BASE
        private DataConnector _dc = null;
        /// <summary>
        /// DataConnector class
        /// </summary>
        public DataConnector DataConnector { get { return _dc ?? (_dc = new DataConnector()); } set { _dc = value; } }

        // caching..
        private CachingHelper _ch = null;
        /// <summary>
        /// Cache. standard of 4 hours.
        /// </summary>
        public CachingHelper Cache { get { return _ch ?? (_ch = new CachingHelper(60 * 4)); } }

        // config
        private OXConfig _cfg = null;
        /// <summary>
        /// Configuratie.
        /// </summary>
        public OXConfig Config { get { return _cfg ?? (_cfg = new OXConfig()); } }

        /// <summary>
        /// Constructor with connector
        /// </summary>
        /// <param name="dc"></param>
        public BaseRT(DataConnector dc)
        {
            DataConnector = dc;
        }

        /// <summary>
        /// Raw destructor.
        /// </summary>
        ~BaseRT()
        {
            if (_dc != null)
            {
                _dc.Dispose();
                _dc = null;
            }
            if (_ch != null)
                _ch = null;
            if (_cfg != null)
                _cfg = null;
        }
#endregion

        /// <summary>
        /// Fills the set by means of an SQL query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sQuery"></param>
        /// <param name="bUseCache"></param>
        /// <returns></returns>
        public IEnumerable<T> Fill<T>(string sQuery, bool bUseCache = false) where T : BaseRTEntity
        {
            // fetch the datatable
            DataTable dt = null;
            string sCacheParam = string.Empty;

            if (bUseCache)
            {
                sCacheParam = StringHelper.MD5(sQuery);
                if (Cache.Contains(new[] { sCacheParam }))
                    dt = Cache.GetCache(new[] { sCacheParam }) as DataTable;
            }
            if (dt == null)
            {
                dt = this.DataConnector.SelectDataTable(sQuery, true);
                if (bUseCache)
                    Cache.SetCache(new[] { sCacheParam }, dt);
            }
            if (Parser.IsDataTableNotEmpty(dt)) // fill the set
                foreach (var dr in dt.Rows) yield return (T)Activator.CreateInstance(typeof(T), new[] { dr, this.DataConnector });
        }
    }

}
