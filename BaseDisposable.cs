using System;
using OXStack.Data;
using OXStack.Config;

namespace OXStack
{
    public class BaseDisposable : IDisposable, IBaseBare
    {
        /// <summary>
        /// Wether Dispose() has been called yet
        /// </summary>
        private bool _bIsDisposed = false;

        private DataConnector _dc = null;
        /// <summary>
        /// DataConnector class
        /// </summary>
        public DataConnector DataConnector { get { return _dc ?? (_dc = new DataConnector()); } set { _dc = value; } }

        private CachingHelper _ch = null;
        /// <summary>
        /// Cache. 4 hours standard.
        /// </summary>
        public CachingHelper Cache { get { return _ch ?? (_ch = new CachingHelper(60*4)); } }

        private OXConfig _cfg = null;
        /// <summary>
        /// Configuration.
        /// </summary>
        public OXConfig Config { get { return _cfg ?? (_cfg = new OXConfig()); } }

        /// <summary>
        /// Normal destructor, for during finalization
        /// </summary>
        ~BaseDisposable() { Dispose(false); }
        /// <summary>
        /// Normal dispose for use in 'using'. All managed resources will be disposed
        /// </summary>
        public void Dispose()
        {
            // dispose managed resources and destruct everything
            Dispose(true);
            // this makes sure the object can't be finalized twice..
            // wich would be overkill, since the first time was when calling the Dispose()
            // method, wich cleaned everything else.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// There are two possibilities to execute this code:
        /// 1) If bDisposing is true. If being called from the code (by 'using' or direct).
        /// In that case everything needs to be cleaned.
        /// 2) From the runtime, when the garbage collector starts to finalize objects.
        /// In the latter case, all managed objects needs to be put down.
        /// </summary>
        /// <param name="bDisposing"></param>
        protected virtual void Dispose(bool bDisposing)
        {
            if (!_bIsDisposed)
            {
                // dispose managed thingies
                if (_dc != null && bDisposing)
                    _dc.Dispose();

                _dc = null;
                _ch = null;
                _cfg = null;

                _bIsDisposed = true;
            }
        }
    }

}
