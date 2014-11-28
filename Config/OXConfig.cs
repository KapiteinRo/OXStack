using System.Collections.Generic;
using System.IO;
using OXStack.Config.IO;

namespace OXStack.Config
{
    public class OXConfig
    {
#region STATIC_CONFIG
        // STATIC CONFIG GO HERE
#endregion

#region DYNAMIC_CONFIG
        /*private DB _db = null;
        /// <summary>
        /// Dynamic configuration for MySQL db.
        /// </summary>
        public DB DB { get { return Load<DB>(ref _db); } }*/


#endregion

#region CONFIG_LOGIC
        /*
         *  BASIC CONFIGURATION FUNCTIONS..
         * 
         */
        
        // configurations
        private OXConfigFiles _configFiles = null;
        // property for configurations, will automatically load
        private OXConfigFiles ConfigFiles { get { return _configFiles ?? (_configFiles = new OXConfigFiles(ConfPath)); } }

        private string _sConfPath = "OXConfig.xml"; // default value
        /// <summary>
        /// Path to main config file.
        /// </summary>
        public string ConfPath
        {
            get
            {
                // if OXConfig is not found in the executing directy, try this one:
                if (!File.Exists(_sConfPath))
                    _sConfPath = System.AppDomain.CurrentDomain.FriendlyName.Replace(".exe", string.Empty).Replace(".vshost", string.Empty) + ".xml";
                // otherwise, look for OXStack.xml
                if (!File.Exists(_sConfPath))
                    _sConfPath = Path.GetFileNameWithoutExtension(this.GetType().Assembly.Location) + ".xml";
                return _sConfPath;
            }
        }


        public OXConfig()
        {
            // for dynamic config types..
        }

        /// <summary>
        /// Generic loading of the configuration
        /// </summary>
        /// <typeparam name="T">Configuration class</typeparam>
        /// <param name="cfg">Reference to private variables</param>
        /// <returns></returns>
        public T Load<T>(ref T cfg) where T : ConfigHelper, new()
        {
            return Load(ref cfg, "main");
        }

        /// <summary>
        /// Generic loading of the configuration
        /// </summary>
        /// <typeparam name="T">Configuration class</typeparam>
        /// <param name="cfg">Reference to private variables</param>
        /// <param name="sConfigFile">path or name of configurationfile</param>
        /// <returns></returns>
        public T Load<T>(ref T cfg, string sConfigFile) where T : ConfigHelper, new()
        {
            if (cfg == null)
                cfg = new T()
                {
                    DefaultStrings = new Dictionary<string, string>(),
                    Node = ConfigFiles.Get(sConfigFile).SelectSingleNode(typeof (T).Name)
                };
            if (cfg.HasExpired())
                cfg = new T()
                {
                    DefaultStrings = cfg.DefaultStrings,
                    Node = ConfigFiles.Get(sConfigFile).SelectSingleNode(typeof (T).Name)
                };
            return cfg;
        }

        /// <summary>
        /// Test configuration.
        /// </summary>
        /// <returns></returns>
        public bool TestConfig()
        {
            return (File.Exists(ConfPath) && ConfigFiles.Get("main") != null);
        }
#endregion

    }
}
