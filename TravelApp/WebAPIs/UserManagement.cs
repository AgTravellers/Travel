using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TravelApp.DataModels;
using TravelApp.Storage.SQL;

namespace TravelApp.WebAPIs
{
    public class UserManagement
    {
        public static UserProfile GetUserProfile(string UserEmailId)
        {
            string userID = String.Empty;
            string imageContainerId = String.Empty;
            using (var SqlRepo = new SQLRepository())
            {
                SqlCommand sqlcmd = new SqlCommand
                {
                    Connection = SqlRepo.GetConnection(),
                    CommandText = "SELECT userid, imagecontainerid FROM userprofile WHERE userid = @userID"
                };
                sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);

                SqlDataReader record = sqlcmd.ExecuteReader();
                if (record.HasRows)
                { 
                    // There should be only one record
                    record.Read();
                    userID = record["userid"].ToString();
                    if (!record.IsDBNull(record.GetOrdinal("imagecontainerid")))
                    {
                        imageContainerId = record["imagecontainerid"].ToString();
                    }
                }
            }
            return new UserProfile(userID, imageContainerId);
        }

        public static void CreateUserProfile(string UserEmailId)
        {
            using (var SqlRepo = new SQLRepository())
            {
                SqlCommand sqlcmd = new SqlCommand
                {
                    Connection = SqlRepo.GetConnection(),
                    CommandText = @"INSERT INTO userprofile VALUES (@userID, null)"
                };
                sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);
                sqlcmd.ExecuteNonQuery();
            }
        }

		public static string CreateImageContainerForUser(string userid)
		{
			// get a unique container guid
			Guid imageContainerGuid = Guid.NewGuid();
			string containerId = "testcontainertwo";// imageContainerGuid.ToString();

			using (var SqlRepo = new SQLRepository())
			{
				SqlCommand sqlcmd = new SqlCommand
				{
					Connection = SqlRepo.GetConnection(),
					CommandText = @"update userprofile set imagecontainerid = @ContainerId where userid = @userid"
				};

				sqlcmd.Parameters.AddWithValue("@userID", userid);
				sqlcmd.Parameters.AddWithValue("@ContainerId", containerId);
				sqlcmd.ExecuteNonQuery();
			}

			// TODO : How do we detect query execution failures ?
			return containerId;
        }

		public static string GetImageContainerIdForUser(string userid)
		{
			UserProfile userProfile = UserManagement.GetUserProfile(userid);
			if (userProfile != null)
			{
				return userProfile.ImageContainerId;
			}

			return null;
		}
    }
}