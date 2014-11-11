using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Text;
using OXStack.Helpers;
using OXStack.RT.Mappers;
using OXStack.Data;

namespace OXStack.RT
{
    /// <summary>
    /// Base class for a class wich is the element of a list
    /// </summary>
    public class BaseRTEntity : Base
    {
        /// <summary>
        /// Constructor, this will fill the class properties from the datarow.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="dc"></param>
        public BaseRTEntity(DataRow dr, DataConnector dc)
        {
            if (dr != null)
                _FillObject(dr);
            this.DataConnector = dc;
        }

        /// <summary>
        /// Fill everything from datarow
        /// </summary>
        /// <param name="dr"></param>
        private void _FillObject(DataRow dr)
        {
            List<string> lsColumns = new List<string>(FetchColumnNames(dr));
            foreach (PropertyInfo info in this.GetType().GetProperties())
            {
                string sPName = info.Name.ToLower(); // name of the property
                bool bHasColumn = lsColumns.Contains(sPName); // if there's a column in the datarow with that propertyname
                if (!bHasColumn)
                {
                    // check if there's an attribute that defines that column
                    object[] attribs = info.GetCustomAttributes(false);
                    foreach (object attrib in attribs)
                    {
                        if (attrib.GetType() == typeof(RTDataColAttribute))
                        {
                            sPName = (attrib as RTDataColAttribute).ColumnName;
                            // re-check if that column exists
                            if (lsColumns.Contains(sPName))
                            {
                                bHasColumn = true;
                                break;
                            }
                        }
                    }
                }

                if (bHasColumn)
                {
                    string sPType = info.PropertyType.ToString();
                    // convert to right type
                    if (sPType == typeof(string).ToString())
                        info.SetValue(this, dr[sPName].ToString(), null);
                    else if (sPType == typeof(int).ToString())
                        info.SetValue(this, Parser.Parse<int>(dr[sPName], -1), null);
                    else if (sPType == typeof(DateTime).ToString())
                        info.SetValue(this, Parser.Parse<DateTime>(dr[sPName], DateTime.MinValue), null);
                    else if (sPType == typeof(bool).ToString())
                        info.SetValue(this, Parser.Parse<bool>(dr[sPName], false), null);
                    else if (sPType == typeof(decimal).ToString())
                        info.SetValue(this, Parser.Parse<decimal>(dr[sPName].ToString().Replace(",", "."), 0), null);
                    // don;t use an else, a property can remain empty as well
                }
            }
        }

        /// <summary>
        /// Fill this object with the first row of the table sTableName where sFieldNameId equals iValueId 
        /// </summary>
        /// <param name="sTableName"></param>
        /// <param name="sFieldNameId"></param>
        /// <param name="iValueId"></param>
        public void Fetch(string sTableName, string sFieldNameId, int iValueId)
        {
            Fetch(sTableName, sFieldNameId, iValueId.ToString());
        }
        /// <summary>
        /// Fill this object with the first row of the table sTableName where sFieldNameId equals 'sValueId'
        /// </summary>
        /// <param name="sTableName"></param>
        /// <param name="sFieldNameId"></param>
        /// <param name="sValueId"></param>
        public void Fetch(string sTableName, string sFieldNameId, string sValueId)
        {
            if (!Parser.IsNumeric(sValueId)) // if it isn't numeric, sanitize
                sValueId = "'" + Parser.Sanitize(sValueId) + "'";
            else // if it is a numeric, clean it up
                sValueId = Parser.ToCleanNumeric(sValueId.Replace(",", "."), ".");

            DataTable dt =
                this.DataConnector.SelectDataTable(
                    "SELECT * FROM " + sTableName + " WHERE " + sFieldNameId + " = " + sValueId, true);

            if (Parser.IsDataTableNotEmpty(dt))
                _FillObject(dt.Rows[0]); // fetch first row
        }

        /// <summary>
        /// Fetch the columns from the datarow
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private IEnumerable<string> FetchColumnNames(DataRow dr)
        {
            foreach (DataColumn c in dr.Table.Columns) yield return c.ColumnName;
        }
    }

}
