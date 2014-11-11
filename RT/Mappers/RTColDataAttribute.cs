using System;

namespace OXStack.RT.Mappers
{
    /// <summary>
    /// Data attribute to map stuff from the datatable
    /// </summary>
    public class RTDataColAttribute : Attribute
    {
        /// <summary>
        /// Desired columnname..
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Map this property with this columnname.
        /// </summary>
        /// <param name="sColName"></param>
        public RTDataColAttribute(string sColName)
        {
            ColumnName = sColName;
        }
    }
}
