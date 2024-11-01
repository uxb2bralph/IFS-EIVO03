using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Web.Configuration;

using Uxnet.Com.Properties;

namespace DataAccessLayer.basis
{
    /// <summary>
    /// </summary>
    public class LayerBase<T> : GenericManager<T> where T : System.Data.Linq.DataContext, new()
    {
        protected object _uid;
        protected bool _useUID = false;
        protected string _orderBy;
        protected SqlTransaction _sqlTran;

        public LayerBase() : base()
        {
        }

        public LayerBase(GenericManager<T> mgr)
          : base(mgr)
        {
        }

        public LayerBase(IDbConnection conn)
          : base(conn)
        {
        }

        ~LayerBase()
        {
        }

        public SqlConnection Connection => (SqlConnection)this._db.Connection;

        protected DataSet dumpSchema(string sqlCmd)
        {
            DataSet dataSet = new DataSet();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd, this.Connection))
                sqlDataAdapter.FillSchema(dataSet, SchemaType.Source);
            return dataSet;
        }

        protected DataSet dumpSchema(SqlCommand sqlCmd)
        {
            DataSet dataSet = new DataSet();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd))
                sqlDataAdapter.FillSchema(dataSet, SchemaType.Source);
            return dataSet;
        }

        protected int contains(string tableName, string columnName, object columnValue)
        {
            try
            {
                this.Connection.Open();
                SqlCommand sqlCommand = new SqlCommand(string.Format("select count(*) from {0} where {1} = @ParamValue ", (object)tableName, (object)columnName), this.Connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ParamValue", columnValue);
                return (int)sqlCommand.ExecuteScalar();
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected void executeSql(string cmdText)
        {
            try
            {
                this.Connection.Open();
                new SqlCommand(cmdText, this.Connection).ExecuteNonQuery();
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected void executeSqlCommand(SqlCommand sqlCmd)
        {
            try
            {
                this.Connection.Open();
                sqlCmd.Connection = this.Connection;
                sqlCmd.ExecuteNonQuery();
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected SqlCommand executeSqlCommand(string storedProcedure, string[] paramName, object[] paramValue)
        {
            try
            {
                this.Connection.Open();
                SqlCommand sqlCommand = !this._useUID ? ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection) : ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection, this._uid);
                for (int index = 0; index < paramName.Length; ++index)
                    sqlCommand.Parameters[paramName[index]].Value = paramValue[index];
                sqlCommand.ExecuteNonQuery();
                return sqlCommand;
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected object executeProcedure(string storedProcedure, string[] paramName, object[] paramValue)
        {
            return this.executeSqlCommand(storedProcedure, paramName, paramValue).Parameters["@RETURN_VALUE"].Value;
        }

        protected SqlCommand executeSqlCommand(string storedProcedure, IDictionary paramValue)
        {
            try
            {
                this.Connection.Open();
                SqlCommand sqlCmd = !this._useUID ? ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection) : ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection, this._uid);
                ModalUtility.AssignCommandParameter(sqlCmd, paramValue);
                sqlCmd.ExecuteNonQuery();
                return sqlCmd;
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected SqlDataReader executeReader(string storedProcedure, IDictionary paramValue)
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();
            SqlCommand sqlCmd = !this._useUID ? ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection) : ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection, this._uid);
            ModalUtility.AssignCommandParameter(sqlCmd, paramValue);
            return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        protected SqlDataReader executeReader(string storedProcedure, params object[] paramValue)
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();
            SqlCommand sqlCmd = !this._useUID ? ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection) : ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection, this._uid);
            ModalUtility.AssignCommandParameter(sqlCmd, paramValue);
            return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        protected object executeProcedure(string storedProcName, IDictionary paramValue) => this.executeSqlCommand(storedProcName, paramValue).Parameters["@RETURN_VALUE"].Value;

        protected SqlCommand executeSqlCommand(string storedProcedure, params object[] cmdParams)
        {
            try
            {
                this.Connection.Open();
                SqlCommand sqlCmd = !this._useUID ? ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection) : ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection, this._uid);
                ModalUtility.AssignCommandParameter(sqlCmd, cmdParams);
                sqlCmd.ExecuteNonQuery();
                return sqlCmd;
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected object executeProcedure(string storedProcedure, params object[] cmdParams) => this.executeSqlCommand(storedProcedure, cmdParams).Parameters["@RETURN_VALUE"].Value;

        protected SqlCommand executeSqlCommand(string storedProcedure)
        {
            try
            {
                this.Connection.Open();
                SqlCommand sqlCommand = !this._useUID ? ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection) : ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection, this._uid);
                sqlCommand.ExecuteNonQuery();
                return sqlCommand;
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected object executeProcedure(string storedProcedure) => this.executeSqlCommand(storedProcedure).Parameters["@RETURN_VALUE"].Value;

        protected SqlCommand executeSqlCommand(string storedProcedure, DataRow row)
        {
            try
            {
                this.Connection.Open();
                SqlCommand sqlCmd = !this._useUID ? ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection) : ModalUtility.InvokeStoredProcedure(storedProcedure, this.Connection, this._uid);
                ModalUtility.AssignCommandParameter(sqlCmd, row);
                sqlCmd.ExecuteNonQuery();
                return sqlCmd;
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected object executeProcedure(string storedProcedure, DataRow row) => this.executeSqlCommand(storedProcedure, row).Parameters["@RETURN_VALUE"].Value;

        public DataSet FillSqlDataSet(string storedProcName, params object[] paramValue) => this.FillSqlDataSet((DataSet)null, (string)null, storedProcName, paramValue);

        private DataSet fillDataSet(DataSet ds, string tableName, SqlCommand sqlCmd, params object[] paramValue)
        {
            if (ds == null)
                ds = new DataSet();
            for (int index = 0; index < sqlCmd.Parameters.Count - 1 && index < paramValue.Length; ++index)
                sqlCmd.Parameters[index + 1].Value = paramValue[index];
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd))
            {
                if (tableName != null)
                    sqlDataAdapter.Fill(ds, tableName);
                else
                    sqlDataAdapter.Fill(ds);
            }
            return ds;
        }

        protected DataSet fillXmlDataSet(DataSet ds, string storedProcName, params object[] paramValue)
        {
            if (ds == null)
                ds = new DataSet();
            try
            {
                this.Connection.Open();
                SqlCommand sqlCommand = ModalUtility.InvokeStoredProcedure(storedProcName, this.Connection);
                for (int index = 0; index < sqlCommand.Parameters.Count - 1 && index < paramValue.Length; ++index)
                    sqlCommand.Parameters[index + 1].Value = paramValue[index];
                object s = sqlCommand.ExecuteScalar();
                if (s != DBNull.Value)
                {
                    StringReader reader = new StringReader((string)s);
                    int num = (int)ds.ReadXml((TextReader)reader, XmlReadMode.IgnoreSchema);
                    reader.Close();
                }
                return ds;
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected DataSet fillSqlDataSet(DataSet ds, string tableName, SqlCommand sqlCmd, params object[] paramValue)
        {
            try
            {
                this.Connection.Open();
                sqlCmd.Connection = this.Connection;
                return this.fillDataSet(ds, tableName, sqlCmd, paramValue);
            }
            finally
            {
                this.Connection.Close();
            }
        }

        protected DataSet fillSqlDataSet(DataSet ds, string tableName, string storedProcName, out SqlCommand sqlCmd, params object[] paramValue)
        {
            try
            {
                this.Connection.Open();
                sqlCmd = ModalUtility.InvokeStoredProcedure(storedProcName, this.Connection);
                return this.fillDataSet(ds, tableName, sqlCmd, paramValue);
            }
            finally
            {
                this.Connection.Close();
            }
        }

        public DataSet FillSqlDataSet(DataSet ds, string tableName, string storedProcName, params object[] paramValue)
        {
            return this.fillSqlDataSet(ds, tableName, storedProcName, out SqlCommand _, paramValue);
        }

        public DataSet FillSqlDataSet(string sqlCmd)
        {
            DataSet dataSet = new DataSet();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd, this.Connection))
                sqlDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public DataSet FillSqlDataSet(string storedProcName, IDictionary paramValue) => this.FillSqlDataSet((DataSet)null, (string)null, storedProcName, paramValue);

        public DataSet FillSqlDataSet(DataSet ds, string tableName, string storedProcName, IDictionary paramValue, int startRecord, int maxRecords, out int recordCount)
        {
            SqlCommand sqlCmd;
            ds = this.fillSqlDataSet(ds, tableName, storedProcName, out sqlCmd, paramValue, startRecord, maxRecords);
            recordCount = (int)sqlCmd.Parameters["@RecordCount"].Value;
            return ds;
        }

        public DataSet FillSqlDataSet(string storedProcName, IDictionary paramValue, int startRecord, int maxRecords, out int recordCount)
        {
            return this.FillSqlDataSet((DataSet)null, (string)null, storedProcName, paramValue, startRecord, maxRecords, out recordCount);
        }

        protected DataSet fillSqlDataSet(DataSet ds, string tableName, SqlCommand sqlCmd, IDictionary paramValue)
        {
            if (ds == null)
                ds = new DataSet();
            try
            {
                this.Connection.Open();
                sqlCmd.Connection = this.Connection;
                ModalUtility.AssignCommandParameter(sqlCmd, paramValue);
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd))
                {
                    if (tableName != null)
                        sqlDataAdapter.Fill(ds, tableName);
                    else
                        sqlDataAdapter.Fill(ds);
                }
            }
            finally
            {
                this.Connection.Close();
            }
            return ds;
        }

        protected DataSet fillSqlDataSet(DataSet ds, string tableName, string storedProcName, out SqlCommand sqlCmd, IDictionary paramValue)
        {
            if (ds == null)
                ds = new DataSet();
            try
            {
                this.Connection.Open();
                sqlCmd = ModalUtility.InvokeStoredProcedure(storedProcName, this.Connection);
                ModalUtility.AssignCommandParameter(sqlCmd, paramValue);
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd))
                {
                    if (tableName != null)
                        sqlDataAdapter.Fill(ds, tableName);
                    else
                        sqlDataAdapter.Fill(ds);
                }
            }
            finally
            {
                this.Connection.Close();
            }
            return ds;
        }

        protected DataSet fillSqlDataSet(DataSet ds, string tableName, string storedProcName, out SqlCommand sqlCmd, IDictionary paramValue, int startRecord, int maxRecords)
        {
            if (ds == null)
                ds = new DataSet();
            try
            {
                this.Connection.Open();
                sqlCmd = ModalUtility.InvokeStoredProcedure(storedProcName, this.Connection, this._orderBy);
                if (paramValue != null)
                    ModalUtility.AssignCommandParameter(sqlCmd, paramValue);
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd))
                {
                    if (tableName != null)
                        sqlDataAdapter.Fill(ds, startRecord, maxRecords, tableName);
                    else
                        sqlDataAdapter.Fill(ds, startRecord, maxRecords, "Table");
                }
            }
            finally
            {
                this.Connection.Close();
            }
            return ds;
        }

        public DataSet FillSqlDataSet(DataSet ds, string tableName, string storedProcName, IDictionary paramValue)
        {
            return this.fillSqlDataSet(ds, tableName, storedProcName, out SqlCommand _, paramValue);
        }

        public DataSet FillSqlDataSet(DataSet ds, string sqlCmd, string tableName)
        {
            if (ds == null)
                ds = new DataSet();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd, this.Connection))
                sqlDataAdapter.Fill(ds, tableName);
            return ds;
        }

        public DataSet FillSqlDataSet(string sqlCmd, out int recordCount)
        {
            DataSet dataSet = this.FillSqlDataSet(sqlCmd);
            recordCount = dataSet.Tables.Count <= 0 ? 0 : dataSet.Tables[0].Rows.Count;
            return dataSet;
        }

        public void Commit()
        {
            if (this._sqlTran == null)
                return;
            this._sqlTran.Commit();
        }

        public void Rollback()
        {
            if (this._sqlTran == null)
                return;
            this._sqlTran.Rollback();
        }
    }
}
