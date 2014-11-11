using System;
using OXStack.Data;
using OXStack.Config;

namespace OXStack
{
    /// <summary>
    /// Raw interface
    /// </summary>
    public interface IBaseBare
    {
        /// <summary>
        /// DataConnector class
        /// </summary>
        DataConnector DataConnector { get; set; }
        /// <summary>
        /// Cache. 4 hours standard
        /// </summary>
        CachingHelper Cache { get; }
        /// <summary>
        /// Configuratie.
        /// </summary>
        OXConfig Config { get; }
    }

}
