using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using OXStack.Helpers;

namespace OXStack.Data
{
    /// <summary>
    /// Data wrapper
    /// </summary>
    public class DataConnector
    {
        // hardcoded connectionstring..
        private string _sConnString = string.Empty;
        // dynamically loaded connectionstring
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_sConnString))
                    _sConnString = string.Empty; // we may throw an exception later
                return _sConnString;
            }
            set { _sConnString = value; }
        }

        /*
         *  configuration
         */

        private SQLiteConnection _conn = null;
        /// <summary>
        /// Connector to connection
        /// </summary>
        private SQLiteConnection Connector
        {
            get { return _conn ?? (_conn = new SQLiteConnection(ConnectionString)); }
        }

        private SQLiteConnection _dbConnection = null;
        /// <summary>
        /// The real connection
        /// </summary>
        public SQLiteConnection Connection
        {
            get { return _dbConnection ?? (_dbConnection = Connector.OpenAndReturn()); }
            set { _dbConnection = value; }
        }

        private SQLiteCommand _dbc = null;
        /// <summary>
        /// Database command
        /// </summary>
        public SQLiteCommand DBC
        {
            get { return _dbc ?? (_dbc = new SQLiteCommand(Connection)); }
        }


        public DataConnector()
        {
            // init?
            // no... no unnecesary cycles to the serviceconfig/encryptor without using it..

        }
        public DataConnector(string sConnString)
        {
            _sConnString = sConnString;
        }

        ~DataConnector()
        {
            // graceful destruction
            Dispose();
            //_srvcConf = null;
            _sConnString = string.Empty;
        }

        /// <summary>
        /// Close connection and destroy DBC
        /// </summary>
        public void Dispose()
        {
            DisposeConnection();
            if (_dbc != null)
                _dbc = null;
        }

        private void DisposeConnection()
        {
            if (_dbConnection != null)
            {
                if (_dbConnection.State != ConnectionState.Closed) _dbConnection.Close();
                _dbConnection = null;
            }
        }

        /// <summary>
        /// Execute reader.. will automatically connect/init
        /// </summary>
        /// <param name="sSQL">SQL code</param>
        /// <returns>data</returns>
        public SQLiteDataReader ExecuteReader(string sSQL)
        {
            // potentially very dangerous
            using (SQLiteCommand cmd = new SQLiteCommand(sSQL, this.Connection))
            {
                return cmd.ExecuteReader();
            }
        }

        /// <summary>
        /// Same as ExecuteNonQuery..
        /// </summary>
        /// <param name="sSQL">SQL query</param>
        /// <returns></returns>
        public int NonQuery(string sSQL)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(sSQL, this.Connection))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Same as ExecuteNonQuery.. but with parameters
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="dsParams"></param>
        /// <returns></returns>
        public int NonQuery(string sSQL, Dictionary<string, object> dsParams)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(sSQL, this.Connection))
            {
                foreach (KeyValuePair<string, object> kvpParams in dsParams)
                {
                    SQLiteParameter param = cmd.CreateParameter();
                    param.Value = kvpParams.Value;
                    param.ParameterName = kvpParams.Key;
                    cmd.Parameters.Add(param);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// ExecuteScalar query
        /// </summary>
        /// <param name="sSql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sSql)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(sSql, this.Connection))
            {
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// ExecuteScalar query with parameters
        /// </summary>
        /// <param name="sSql"></param>
        /// <param name="dsParams"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sSql, Dictionary<string, object> dsParams)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(sSql, this.Connection))
            {
                foreach (KeyValuePair<string, object> kvpParams in dsParams)
                {
                    SQLiteParameter param = cmd.CreateParameter();
                    param.Value = kvpParams.Value;
                    param.ParameterName = kvpParams.Key;
                    cmd.Parameters.Add(param);
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Selects highest id in a table..
        /// </summary>
        /// <param name="sColumn">Column of the id</param>
        /// <param name="sTable">Tablename</param>
        /// <returns>Returns -1 if not found or something wrong..</returns>
        public int SelectMax(string sColumn, string sTable)
        {
            int iRet = -1;

            string sQuery = "SELECT MAX(" + sColumn + ") AS _selectmax FROM " + sTable;
            DataTable dt = this.SelectDataTable(sQuery);

            if (Parser.IsDataTableNotEmpty(dt))
                iRet = Parser.Parse<int>(dt.Rows[0]["_selectmax"], -1);


            return iRet;
        }

        /// <summary>
        /// The same as ExecuteReader() except it returns a nice DataTable instead of an archaic DataReader
        /// </summary>
        /// <param name="sSQL">SQL query</param>
        /// <returns>data</returns>
        public DataTable SelectDataTable(string sSQL)
        {
            return SelectDataTable(sSQL, false);
        }
        /// <summary>
        /// The same as ExecuteReader() except it returns a nice DataTable instead of an archaic DataReader
        /// </summary>
        /// <param name="sSQL">SQL query</param>
        /// <param name="bAutoDispose">dispose on finish?</param>
        /// <returns></returns>
        public DataTable SelectDataTable(string sSQL, bool bAutoDispose)
        {
            SQLiteDataReader dr = this.ExecuteReader(sSQL);

            DataTable dtSchema = dr.GetSchemaTable();
            List<DataColumn> liCols = new List<DataColumn>();
            DataTable dtRet = null;

            if (dtSchema != null)
            {
                dtRet = new DataTable();
                // fetch columns..
                foreach (DataRow row in dtSchema.Rows)
                {
                    string sColName = row["ColumnName"].ToString();
                    DataColumn dcColumn = new DataColumn(sColName, (Type)(row["DataType"]));
                    //dcColumn.Unique = (bool)row["IsUnique"]; // <-- THIS IS NASTY
                    dcColumn.AllowDBNull = (bool)row["AllowDBNull"];
                    dcColumn.AutoIncrement = (bool)row["IsAutoIncrement"];

                    liCols.Add(dcColumn);
                    dtRet.Columns.Add(dcColumn);
                }

                // populate datatable
                while (dr.Read())
                {
                    DataRow dRow = dtRet.NewRow();
                    for (int i = 0; i < liCols.Count; i++)
                    {
                        dRow[((DataColumn)liCols[i])] = dr[i];
                    }
                    dtRet.Rows.Add(dRow);
                }

                // close datareader
                if (!dr.IsClosed)
                {
                    dr.Close();
                    dr = null;
                }
            }

            
            //DisposeConnection();
            // autodispose mag
            if (bAutoDispose)
                Dispose();

            return dtRet;
        }

    }

}
