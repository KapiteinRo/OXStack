using System;
using OXStack.Data;
using OXStack.Config;

namespace OXStack
{
    /// <summary>
    /// Basic class with basic functions
    /// </summary>
    public class Base : IBaseBare
    {
        private DataConnector _dc = null;
        /// <summary>
        /// DataConnector class
        /// </summary>
        public DataConnector DataConnector { get { return _dc ?? (_dc = new DataConnector()); } set { _dc = value; } }

        // caching..
        private CachingHelper _ch = null;
        /// <summary>
        /// Cache. 4 hours standard
        /// </summary>
        public CachingHelper Cache { get { return _ch ?? (_ch = new CachingHelper(60 * 4)); } }

        // config
        private OXConfig _cfg = null;
        /// <summary>
        /// Configuration
        /// </summary>
        public OXConfig Config { get { return _cfg ?? (_cfg = new OXConfig()); } }

        /// <summary>
        /// Raw destructor.
        /// </summary>
        ~Base()
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
    }

}
