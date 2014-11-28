using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace OXStack.Config.IO
{
    /// <summary>
    /// Dynamic Config File
    /// </summary>
    public class OXConfigFile
    {
        /// <summary>
        /// Configuration ID..
        /// </summary>
        public string CID { get; set; }
        /// <summary>
        /// Path to XML
        /// </summary>
        public string Path { get; set; }

        // date of last changed
        private DateTime _dtLastChanged = DateTime.MinValue;
        // XML config
        private XmlElement _xmlConf = null;

        /// <summary>
        /// config file, will automatically reload if something has changed..
        /// </summary>
        public XmlElement XmlConfig
        {
            get
            {
                if (_xmlConf == null || HasConfigChanged()) LoadConfig();
                return _xmlConf;
            }
        }

        // check if configfile has changed..
        private bool HasConfigChanged()
        {
            if (File.Exists(Path) && File.GetLastWriteTime(Path) != _dtLastChanged)
                    return true;
            return false;
        }

        // (re)load configfile...
        private void LoadConfig()
        {
            if (File.Exists(Path))
            {
                _dtLastChanged = File.GetLastWriteTime(Path);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(File.ReadAllText(Path));
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
        /// ctor
        /// </summary>
        /// <param name="sConfigPath"></param>
        /// <param name="sCID"></param>
        public OXConfigFile(string sConfigPath, string sCID)
        {
            CID = sCID;
            Path = sConfigPath;
        }
    }

    /// <summary>
    /// List of config files.. Main config is the first element
    /// </summary>
    public class OXConfigFiles : List<OXConfigFile>
    {
        /// <summary>
        /// Load, put the main config in the first element.
        /// </summary>
        /// <param name="sMainConfigPath"></param>
        public OXConfigFiles(string sMainConfigPath)
        {
            // create a main-config
            Add(sMainConfigPath, "main");
        }

        // add to list, unless it exists
        private void Add(string sConfigPath, string sCID)
        {
            if (!Exists(sCID))
                this.Add(new OXConfigFile(sConfigPath, sCID));
        }

        /// <summary>
        /// Add config to list
        /// </summary>
        /// <param name="sConfigPath"></param>
        public void Add(string sConfigPath)
        {
            Add(sConfigPath, sConfigPath);
        }

        /// <summary>
        /// Does it exist?
        /// </summary>
        /// <param name="sCID">case-insensitive path or ID</param>
        /// <returns></returns>
        public bool Exists(string sCID)
        {
            return this.Exists(x => x.CID.ToLower() == sCID.ToLower());
        }

        /// <summary>
        /// Fetch config XML, will fetch the first in the list if it can't find the desired one.
        /// </summary>
        /// <param name="sCID"></param>
        /// <returns></returns>
        public XmlElement Get(string sCID)
        {
            if (!Exists(sCID))
                Add(sCID);
            int iIndex = FindIndex(x => x.CID.ToLower() == sCID.ToLower());
            return this[iIndex < 0 ? 0 : iIndex].XmlConfig;
        }
    }
}
