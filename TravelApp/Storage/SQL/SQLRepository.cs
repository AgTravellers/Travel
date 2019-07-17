using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TravelApp.Storage.SQL
{
    // This class will serve as the gateway for SQL Connections. Since ADO.net offers
    // connection pooling, we just need to open and close the connections as and when
    // required.
    // TODO: Explore max pool size and min pool size
    public class SQLRepository : IDisposable
    {
        private SqlConnection SQLConn;
        private readonly string DBConnString = "Server=tcp:ta-sqlserver.database.windows.net,1433;Initial Catalog=ta-sqldb;Persist Security Info=False;User ID=ta-sqluser;Password=hujugshuruholo2019$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public SQLRepository()
        {
            SQLConn = new SqlConnection(DBConnString);
            SQLConn.Open();
        }

        public SqlConnection GetConnection()
        {
            return SQLConn;
        }

        public SqlDataReader GetRecords(string query)
        {
            SqlCommand oCmd = new SqlCommand(query, SQLConn);
            return oCmd.ExecuteReader();
        }

        public void Dispose()
        {
            SQLConn.Close();
        }

    }
}