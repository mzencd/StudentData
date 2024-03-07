using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace StudentData
{
    internal class DataModel : IDisposable
    {
        protected SQLiteConnection Connection { get; set; }

        public SQLiteConnection GetConnection()
        {
            return Connection;
        }

        protected void SetupConnection()
        {
            try
            {
                if (Connection == null)
                {
                    string directory = Directory.GetCurrentDirectory(); //.Replace("\\bin\\Debug", "");
                    string dbFileName = directory + @"\Database.db";
                    string connstring = $"Data Source={dbFileName};Mode=ReadWriteCreate";

                    Connection = new SQLiteConnection(connstring);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private bool _isDisposed = false;

        public DataModel()
        {
            SetupConnection();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            Connection.Close();

            if (disposing)
            {
                Connection.Dispose();
            }

            Connection = null;
            _isDisposed = true;
        }

        ~DataModel()
        {
            Dispose(false);
        }

        public SqlResult SqlQuery(string strQuery, CommonSqlParameter[] parameters, string tableName)
        {
            SqlResult result = new SqlResult();
            DataSet ds = null;

            try
            {
                if (string.IsNullOrEmpty(strQuery))
                {
                    result.DataSet = null;
                    result.HasError = true;
                    result.Message = "Query string not applied.";
                    result.ReturnValue = 0;
                    return result;
                }

                SQLiteDataAdapter adapter = new SQLiteDataAdapter();
                adapter.SelectCommand = new SQLiteCommand(strQuery, Connection);

                if (parameters != null)
                {
                    foreach (CommonSqlParameter parameter in parameters)
                    {
                        SqlParameter sqlParam = new SqlParameter(parameter.Name, parameter.DBType)
                        {
                            Value = parameter.Value
                        };
                        adapter.SelectCommand.Parameters.Add(sqlParam);
                    }
                }

                ds = new DataSet();
                if (!string.IsNullOrEmpty(tableName))
                {
                    adapter.FillSchema(ds, SchemaType.Source, tableName);
                    result.ReturnValue = adapter.Fill(ds, tableName);
                }
                else
                {
                    adapter.FillSchema(ds, SchemaType.Source);
                    result.ReturnValue = adapter.Fill(ds);
                }
                result.DataSet = ds;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public SqlResult UpdateOnDataTable(DataTable dataTable, string strQuery, CommonSqlParameter[] parameters)
        {
            SqlResult result = new SqlResult();

            try
            {
                if (dataTable == null || string.IsNullOrEmpty(strQuery))
                {
                    result.DataSet = null;
                    result.HasError = true;
                    result.Message = "DataTable OR Query string not applied.";
                    result.ReturnValue = 0;
                    return result;
                }

                SQLiteDataAdapter adapter = new SQLiteDataAdapter();
                adapter.SelectCommand = new SQLiteCommand(strQuery, Connection);

                if (parameters != null)
                {
                    foreach (CommonSqlParameter parameter in parameters)
                    {
                        SqlParameter sqlParam = new SqlParameter(parameter.Name, parameter.DBType)
                        {
                            Value = parameter.Value
                        };
                        adapter.SelectCommand.Parameters.Add(sqlParam);
                    }
                }

                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);

                adapter.ContinueUpdateOnError = true;
                result.ReturnValue = adapter.Update(dataTable);

                DataSet dataSet = dataTable.DataSet;
                if (dataSet == null)
                {
                    dataSet = new DataSet();
                    dataSet.Tables.Add(dataTable);
                }

                result.DataSet = dataSet;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public SqlResult ExecuteNonQuery(string strSQL, CommonSqlParameter[] parameters)
        {
            SqlResult result = new SqlResult();

            try
            {
                if (string.IsNullOrEmpty(strSQL))
                {
                    result.DataSet = null;
                    result.HasError = true;
                    result.Message = "SQL string not applied.";
                    result.ReturnValue = 0;
                    return result;
                }

                SQLiteCommand command = new SQLiteCommand(strSQL, Connection);

                if (parameters != null)
                {
                    foreach (CommonSqlParameter parameter in parameters)
                    {
                        SqlParameter sqlParam = new SqlParameter(parameter.Name, parameter.DBType)
                        {
                            Value = parameter.Value
                        };
                        command.Parameters.Add(sqlParam);
                    }
                }

                Connection.Open();
                result.ReturnValue = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public SqlResult ExecuteScalar(string strSQL, CommonSqlParameter[] parameters)
        {
            SqlResult result = new SqlResult();

            try
            {
                if (string.IsNullOrEmpty(strSQL))
                {
                    result.DataSet = null;
                    result.HasError = true;
                    result.Message = "SQL string not applied.";
                    result.ReturnValue = 0;
                    return result;
                }

                SQLiteCommand command = new SQLiteCommand(strSQL, Connection);

                if (parameters != null)
                {
                    foreach (CommonSqlParameter parameter in parameters)
                    {
                        SqlParameter sqlParam = new SqlParameter(parameter.Name, parameter.DBType)
                        {
                            Value = parameter.Value
                        };
                        command.Parameters.Add(sqlParam);
                    }
                }

                Connection.Open();
                result.ReturnValue = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public class CommonSqlParameter
        {
            public string Name { get; set; }

            public SqlDbType DBType { get; set; }

            public Object Value { get; set; }
        }

        public class SqlResult
        {
            public object ReturnValue { get; set; }

            public DataSet DataSet { get; set; }

            public bool HasError { get; set; }

            public string Message { get; set; }
        }
    }
}
