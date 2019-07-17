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
    }
}