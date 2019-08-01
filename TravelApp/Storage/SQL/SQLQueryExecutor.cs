using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TravelApp.Storage.SQL
{
	public class SQLQueryExecutor
	{
		private SQLQueryExecutor sqlQueryExecutor;
		private readonly string DBConnString = "Server=tcp:ta-sqlserver.database.windows.net,1433;Initial Catalog=ta-sqldb;Persist Security Info=False;User ID=ta-sqluser;Password=hujugshuruholo2019$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		private SQLQueryExecutor()
		{
			
        }

		public SQLQueryExecutor GetInstance()
		{
			if (sqlQueryExecutor == null)
			{
				sqlQueryExecutor = new SQLQueryExecutor();
			}

			return sqlQueryExecutor;
		}

		public  SqlDataReader GetUserProfile(string UserEmailId)
		{
			//string userID = String.Empty;
			//string imageContainerId = String.Empty;
			//using (var SqlRepo = new SQLRepository())
			//{
			//	SqlCommand sqlcmd = new SqlCommand
			//	{
			//		Connection = SqlRepo.GetConnection(),
			//		CommandText = "SELECT userid, imagecontainerid FROM userprofile WHERE userid = @userID"
			//	};
			//	sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);

			//	SqlDataReader record = sqlcmd.ExecuteReader();
			//	if (record.HasRows)
			//	{
			//		// There should be only one record
			//		record.Read();
			//		userID = record["userid"].ToString();
			//		if (!record.IsDBNull(record.GetOrdinal("imagecontainerid")))
			//		{
			//			imageContainerId = record["imagecontainerid"].ToString();
			//		}
			//	}
			//}
			//return new UserProfile(userID, imageContainerId);
			return null;
		}


	}
}