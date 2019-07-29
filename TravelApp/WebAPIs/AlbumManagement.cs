using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TravelApp.Storage.SQL;
using TravelApp.DataModels;

namespace TravelApp.WebAPIs
{
    public class AlbumManagement
    {
        public static List<AlbumInfo> GetAlbumsForUser(String UserEmailId)
        {
            List<AlbumInfo> albums = new List<AlbumInfo>();

            using (var SqlRepo = new SQLRepository())
            {
                SqlCommand sqlcmd = new SqlCommand
                {
                    Connection = SqlRepo.GetConnection(),
                    CommandText = "SELECT containerid, albumname FROM imagecontainers WHERE userid = @userID"
                };
                sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);

                SqlDataReader record = sqlcmd.ExecuteReader();
                while (record.Read())
                {
                    albums.Add(new AlbumInfo(record["albumname"].ToString(), record["containerid"].ToString()));
                }
            }
            return albums;
        }

        public static void CreateAlbum(String UserEmailId, string AlbumName)
        {
            Random rnd = new Random();
            int number = rnd.Next(1, 100);
            string containername = "testcontainer" + number.ToString();
            using (var SqlRepo = new SQLRepository())
            {
                SqlCommand sqlcmd = new SqlCommand
                {
                    Connection = SqlRepo.GetConnection(),
                    CommandText = "INSERT INTO imagecontainers VALUES (@containerName, @userID, @albumName, null)"
                };
                sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);
                sqlcmd.Parameters.AddWithValue("@containerName", containername);
                sqlcmd.Parameters.AddWithValue("@albumName", AlbumName);
                sqlcmd.ExecuteNonQuery();
            }
        }
    }
}