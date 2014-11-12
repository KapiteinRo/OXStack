using System;
using System.Collections.Generic;
using System.Xml;

namespace OXStack.Config
{
    /// <summary>
    /// Parent class of dynamic-loaded configtypes...
    /// </summary>
    public class ConfigHelper
    {
        // all the defaults, will be reset automatically when a configfile is loaded..
        private Dictionary<string, string> _dsDefaults = null;
        public Dictionary<string,string> DefaultStrings
        {
            get { return _dsDefaults ?? (_dsDefaults = new Dictionary<string, string>()); }
            set { _dsDefaults = value; }
        }

        // the selected XmlNode
        private XmlNode _xmlNode = null;
        public XmlNode Node { get { return _xmlNode; } set { _xmlNode = value; } }
        // the last time the file has been checked..
        private DateTime _dtLastChecked = DateTime.MinValue;
        
        /// <summary>
        /// Generic constructor
        /// </summary>
        public ConfigHelper()
        {
            _dtLastChecked = DateTime.Now;
        }

        /// <summary>
        /// Constructs base class of any dynamic-loaded configuration..
        /// </summary>
        /// <param name="xmlElem">input xml element</param>
        /// <param name="sParentNode">parent node to look for</param>
        public ConfigHelper(XmlElement xmlElem, string sParentNode)
        {
            _xmlNode = xmlElem.SelectSingleNode(sParentNode);
            _dtLastChecked = DateTime.Now;
        }

        /// <summary>
        /// Constructs base class of any dynamic-loaded configuration, and copies the defaults of an old class
        /// </summary>
        /// <param name="xmlElem"></param>
        /// <param name="sParentNode"></param>
        /// <param name="dsPrevDefaults"></param>
        public ConfigHelper(XmlElement xmlElem, string sParentNode, Dictionary<string, string> dsPrevDefaults)
        {
            _xmlNode = xmlElem.SelectSingleNode(sParentNode);
            _dtLastChecked = DateTime.Now;
            DefaultStrings = dsPrevDefaults;
        }
        
        /// <summary>
        /// Check if this configuration has expired, it will return true after 3 minutes of use..
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            if (_dtLastChecked == DateTime.MinValue)
                _dtLastChecked = DateTime.Now;
            return _dtLastChecked.AddMinutes(3) < DateTime.Now;
        }

        /// <summary>
        /// Get string/integer/boolean from config by sKey, returns defaultValue if not found / errors in config..
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T Get<T>(string sKey, T defaultValue)
        {
            string sValue = string.Empty;
            // try to fetch the string from the XML first
            try
            {
                XmlNode xn = _xmlNode.SelectSingleNode(sKey);
                if (xn != null)
                {
                    sValue = xn.InnerText;
                    // if found, put it in default
                    SetDefault(sKey, sValue);
                }
            }
            catch
            {
                // ack..
            }

            // if sValue empty, check if there's a default present
            // wich might be the last known value of that string..
            if (string.IsNullOrEmpty(sValue) && DefaultStrings.ContainsKey(sKey))
                sValue = DefaultStrings[sKey];

            // is it an integer?
            if (typeof(T) == typeof(int))
            {
                int iRet = 0;
                if (!string.IsNullOrEmpty(sValue) && Int32.TryParse(sValue, out iRet))
                    return (T) Convert.ChangeType(iRet, typeof (int));
                return defaultValue;
            } 
            // is it a bool?
            if(typeof(T) == typeof(bool))
            {
                if (!string.IsNullOrEmpty(sValue))
                {
                    bool bRet = (sValue == "1");
                    return (T) Convert.ChangeType(bRet, typeof (bool));
                }
                return defaultValue;
            }

            // is it a string?
            if (typeof(T) == typeof(string))
            {
                if (!string.IsNullOrEmpty(sValue))
                    return (T) Convert.ChangeType(sValue, typeof (string));
                // decode defaultvalue
                string sTmp = (string)Convert.ChangeType(defaultValue, typeof(string));
                return (T) Convert.ChangeType(sTmp, typeof (string));
            }

            // haven't got the foggiest..
            return defaultValue;
        }



        /// <summary>
        /// Set default value for sKey
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="sValue"></param>
        private void SetDefault(string sKey, string sValue)
        {
            if (DefaultStrings.ContainsKey(sKey)) DefaultStrings[sKey] = sValue;
            else DefaultStrings.Add(sKey, sValue);
        }

    }

}
