using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace OXStack.Helpers
{
    /// <summary>
    /// Contains parsings from and to Dataview-specific types
    /// </summary>
    public class Parser
    {
        // no ctor.. only statics

        public static T Parse<T>(object value, T defaultValue)
        {
            if (value == null)
                return defaultValue;

            // start with common types first..
            if (typeof(T) == typeof(int))
            {
                int iRet = 0;
                if (int.TryParse(value.ToString(), out iRet))
                    return (T)Convert.ChangeType(iRet, typeof(int));
            }
            else if (typeof(T) == typeof(decimal))
            {
                decimal dRet = 0;
                //CultureInfo ci = CultureInfo.CreateSpecificCulture("nl-NL");
                //if (value.ToString().IndexOf(".") > -1)
                //    ci = CultureInfo.CreateSpecificCulture("en-US");
                //if (decimal.TryParse(value.ToString(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, ci, out dRet))
                if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out dRet))
                    return (T)Convert.ChangeType(dRet, typeof(decimal));
            }
            else if (typeof(T) == typeof(bool))
            {
                bool bRet = (bool)Convert.ChangeType(defaultValue, typeof(bool));
                if (value.ToString().Trim() == "1") bRet = true;
                if (value.ToString().ToLower().Trim() == "true") bRet = true;
                return (T)Convert.ChangeType(bRet, typeof(bool));
            }
            else if (typeof(T) == typeof(DateTime))
            {
                DateTime dtRet = DateTime.MinValue;
                if (value.ToString().IndexOf("0000") > -1)
                    return defaultValue;
                if (DateTime.TryParse(value.ToString(), out dtRet))
                    return (T)Convert.ChangeType(dtRet, typeof(DateTime));
            }
            else if (typeof(T) == typeof(double))
            {
                double dRet = 0;
                if (double.TryParse(value.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dRet))
                    return (T)Convert.ChangeType(dRet, typeof(double));
            }
            else if (typeof(T).IsEnum)
            {
                // parse all custom enums
                // WARNING: never use Enum.IsDefined cuz it hogs performance
                string sValue = value.ToString();
                if (!string.IsNullOrEmpty(sValue))
                {
                    // fast approach
                    int iFast = -1;
                    if (int.TryParse(sValue, out iFast))
                        if (Enum.IsDefined(typeof(T), iFast)) // performance hog, but it runs when there's an int
                            return (T)Enum.ToObject(typeof(T), iFast);

                    foreach (string sName in Enum.GetNames(typeof(T)))
                    {
                        if (sName.ToLower().Equals(sValue.ToLower()))
                        {
                            return (T)Enum.Parse(typeof(T), sValue, true);
                        }
                    }

                    if (Enum.IsDefined(typeof(T), Convert.ChangeType(value, (Enum.GetUnderlyingType(typeof(T))))))
                    {
                        return (T)Enum.Parse(typeof(T), value.ToString(), true);
                    }
                }
            }

            return defaultValue;
        }

        public static string XmlNodeParse(XmlElement xmlElement, string sName)
        {
            return XmlNodeParse(xmlElement, sName, string.Empty);
        }
        public static string XmlNodeParse(XmlElement xmlElement, string sName, string sDefaultValue)
        {
            string sRet = sDefaultValue;

            XmlNode xnNode = xmlElement.SelectSingleNode(sName);

            if (xnNode != null)
                sRet = xnNode.InnerText;
            return sRet;
        }
        public static string XmlNodeParse(XmlNode xmlNode, string sName, string sDefaultValue)
        {
            string sRet = sDefaultValue;

            XmlNode xnNode = xmlNode.SelectSingleNode(sName);

            if (xnNode != null)
                sRet = xnNode.InnerText;
            return sRet;
        }

        public static int XmlNodeParse(XmlElement xmlElement, string sName, int iDefaultValue)
        {
            int iRet = iDefaultValue;

            XmlNode xnNode = xmlElement.SelectSingleNode(sName);

            if (xnNode != null)
                iRet = Parse(xnNode.InnerText, iDefaultValue);
            return iRet;
        }
        public static int XmlNodeParse(XmlNode xmlNode, string sName, int iDefaultValue)
        {
            int iRet = iDefaultValue;

            XmlNode xnNode = xmlNode.SelectSingleNode(sName);

            if (xnNode != null)
                iRet = Parse(xnNode.InnerText, iDefaultValue);
            return iRet;
        }
        public static bool XmlNodeParse(XmlNode xmlNode, string sName, bool bDefaultValue)
        {
            bool bRet = bDefaultValue;

            XmlNode xnNode = xmlNode.SelectSingleNode(sName);

            if (xnNode != null)
                bRet = Parse(xnNode.InnerText, bDefaultValue);
            return bRet;
        }
        public static decimal XmlNodeParse(XmlNode xmlNode, string sName, decimal dDefaultValue)
        {
            decimal dRet = dDefaultValue;

            XmlNode xnNode = xmlNode.SelectSingleNode(sName);

            if (xnNode != null)
                dRet = Parse(xnNode.InnerText, dDefaultValue);

            return dRet;
        }

        public static bool IsNumeric(string sTest)
        {
            sTest =
                sTest.Replace("-", string.Empty).Replace(",", string.Empty).Replace(".", string.Empty).Replace(" ",
                                                                                                               string.
                                                                                                                   Empty)
                    .Replace("%", string.Empty);
            int iTest = -1;

            if (!int.TryParse(sTest.Trim(), out iTest))
                iTest = -1;

            return (iTest > -1);
        }

        public static bool IsDataTableNotEmpty(DataTable dt)
        {
            return (dt != null && dt.Rows.Count > 0);
        }

        public static string ToCleanNumeric(string sValue)
        {
            return ToCleanNumeric(sValue, ",");
        }
        public static string ToCleanNumeric(string sValue, string sFloat)
        {
            StringBuilder sb = new StringBuilder();
            string sAllowed = "-0123456789" + sFloat;

            for (int i = 0; i < sValue.Length; i++)
                if (sAllowed.Contains(sValue.Substring(i, 1)))
                    sb.Append(sValue.Substring(i, 1));

            return sb.ToString();
        }

        /// <summary>
        /// Sanitize input voor MySQL queries..
        /// </summary>
        /// <param name="sInput">input data</param>
        /// <returns>sanitized input</returns>
        public static string Sanitize(string sInput)
        {
            if (string.IsNullOrEmpty(sInput)) return sInput;
            return Regex.Replace(sInput, @"[\r\n\x00\x1a\\'""]", @"\$0");
        }

        /*
         *  SERIALIZERS
         */
        /// <summary>
        /// Serialize an object to XML (WARNING: this object MUST have a constructor)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize<T>(T value)
        {
            if (value == null) return string.Empty;
            try
            {
                MemoryStream ms = new MemoryStream();
                // we use memorystream, to bypass the standard UTF-16 of textstream..
                using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(T));
                    xSer.Serialize(xw, value);
                }
                ms.Seek(0L, SeekOrigin.Begin);

                return (new StreamReader(ms)).ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Serialize a list of objects to XML (WARNING: every object needs to have an constructor)
        /// HINT: in case sRootNodeName is empty, the function will assume that the rootname the name of T
        /// plus an 's' (so if T is Beer than the rootname becomes Beers)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize<T>(List<T> value, string sRootNodeName)
        {
            if (value == null) return string.Empty;
            try
            {
                MemoryStream ms = new MemoryStream();
                // we use memorystream, to bypass the standard UTF-16 of textstream..
                using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
                {
                    // let's do some magic.. the XmlNode wich contains the list, has the same name as T, plus 's'.
                    if (string.IsNullOrEmpty(sRootNodeName))
                        sRootNodeName = typeof(T).Name + "s";

                    XmlSerializer xSer = new XmlSerializer(value.GetType(), new XmlRootAttribute(sRootNodeName));
                    xSer.Serialize(xw, value);
                }
                ms.Seek(0L, SeekOrigin.Begin);

                return (new StreamReader(ms)).ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Unserialize a list of objects from XML (WARNING: every object needs to have an constructor)
        /// HINT: in case sRootNodeName is empty, the function will assume that the rootname the name of T
        /// plus an 's' (so if T is Beer than the rootname becomes Beers)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sDoc"></param>
        /// <param name="sRootNodeName"></param>
        /// <returns></returns>
        public static List<T> Unserialize<T>(string sDoc, string sRootNodeName)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.LoadXml(sDoc);
                // try figure out the rootnode name
                // if empty, this becomes name of T plus s.
                if (string.IsNullOrEmpty(sRootNodeName))
                    sRootNodeName = typeof(T).Name + "s";

                XmlSerializer xSer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute(sRootNodeName));

                return (List<T>)xSer.Deserialize(new XmlNodeReader(xDoc.DocumentElement));
            }
            catch
            {
                // nothing
                return null;
            }
        }

        /// <summary>
        /// Unserialize an object from XML (WARNING: every object needs to have an constructor)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sXml"></param>
        /// <returns></returns>
        public static T Unserialize<T>(string sXml)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.LoadXml(sXml);
                XmlSerializer xSer = new XmlSerializer(typeof(T));
                return (T)xSer.Deserialize(new XmlNodeReader(xDoc.DocumentElement));
            }
            catch
            {
                // return empty
                return (T)Activator.CreateInstance(typeof(T));
            }
        }
    }

}
