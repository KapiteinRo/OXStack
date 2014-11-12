using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

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
        // dynamic config loading
        private DateTime _dtLastChanged = DateTime.MinValue;
        private XmlElement _xmlConf = null;

        // config file, will automatically reload if something has changed..
        private XmlElement XmlConfig
        {
            get
            {
                if (_xmlConf == null || HasConfigChanged()) LoadConfig();
                return _xmlConf;
            }
        }

        // path to config file
        private string _sConfPath = "OXConfig.xml";
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

        // check if configfile has changed..
        private bool HasConfigChanged()
        {
            if (File.Exists(ConfPath))
                if (File.GetLastWriteTime(ConfPath) != _dtLastChanged)
                    return true;
            return false;
        }

        // (re)load configfile...
        private void LoadConfig()
        {
            if (File.Exists(ConfPath))
            {
                _dtLastChanged = File.GetLastWriteTime(ConfPath);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(File.ReadAllText(ConfPath));
                _xmlConf = xmlDoc.DocumentElement;
            }
            else
            {
                // create dummy
                _dtLastChanged = DateTime.Now;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<foo/>");
                _xmlConf = xmlDoc.DocumentElement;
            }
        }

        /// <summary>
        /// Generic loading of the configuration
        /// </summary>
        /// <typeparam name="T">Configuration class</typeparam>
        /// <param name="cfg">Reference to private variables</param>
        /// <returns></returns>
        public T Load<T>(ref T cfg) where T : ConfigHelper, new()
        {
            if (cfg == null)
                cfg = new T()
                          {
                              DefaultStrings = new Dictionary<string, string>(),
                              Node = this.XmlConfig.SelectSingleNode(typeof(T).Name)
                          };
            if (cfg.HasExpired())
                cfg = new T() { DefaultStrings = cfg.DefaultStrings, Node = this.XmlConfig.SelectSingleNode(typeof(T).Name) };
            return cfg;
        }

        /// <summary>
        /// Test configuration.
        /// </summary>
        /// <returns></returns>
        public bool TestConfig()
        {
            return (File.Exists(ConfPath) && this.XmlConfig != null);
        }
#endregion

    }
}
