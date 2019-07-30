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
                    CommandText = "SELECT containerid, albumname, lastimageindex FROM imagecontainers WHERE userid = @userID"
                };
                sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);

                SqlDataReader record = sqlcmd.ExecuteReader();
                while (record.Read())
                {
                    albums.Add(new AlbumInfo(record["albumname"].ToString(), record["containerid"].ToString(), record["lastimageindex"].ToString()));
                }
            }

            return albums;
        }

        public static AlbumInfo CreateAlbum(String UserEmailId, string AlbumName)
        {
            Random rnd = new Random();
            int number = rnd.Next(1, 100);
            string containername = "testcontainer" + number.ToString();
			string lastImageIndex = "0";
			AlbumInfo albumInfo = new AlbumInfo(AlbumName, containername, lastImageIndex);
            using (var SqlRepo = new SQLRepository())
            {
                SqlCommand sqlcmd = new SqlCommand
                {
                    Connection = SqlRepo.GetConnection(),
                    CommandText = "INSERT INTO imagecontainers VALUES (@containerName, @userID, @albumName, null, @lastImageIndex)"
                };
                sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);
                sqlcmd.Parameters.AddWithValue("@containerName", albumInfo.ContainerName);
                sqlcmd.Parameters.AddWithValue("@albumName", albumInfo.AlbumName);
				sqlcmd.Parameters.AddWithValue("@lastImageIndex", albumInfo.LastImageIndex);
				sqlcmd.ExecuteNonQuery();
            }

			return albumInfo;
        }

		public static void UpdateLastImageIndex(string containerId, int lastImageIndex)
		{
			using (var SqlRepo = new SQLRepository())
			{
				SqlCommand sqlcmd = new SqlCommand
				{
					Connection = SqlRepo.GetConnection(),
					CommandText = "UPDATE imagecontainers set lastimageindex = @lastImageIndex where containerid = @containerId"
				};
				sqlcmd.Parameters.AddWithValue("@lastImageIndex", lastImageIndex);
				sqlcmd.Parameters.AddWithValue("@containerId", containerId);
				sqlcmd.ExecuteNonQuery();
			}
		}
	}
}