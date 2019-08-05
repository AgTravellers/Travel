using System;
using System.Collections.Generic;
using TravelApp.Storage.ImageStore;
using TravelApp.DataModels;
using System.Linq;

namespace TravelApp.WebAPIs
{
    public class AlbumManagement
    {
        //public static List<AlbumInfo> GetAlbumsForUser(String UserEmailId)
        //{
        //    List<AlbumInfo> albums = new List<AlbumInfo>();

        //    using (var SqlRepo = new SQLRepository())
        //    {
        //        SqlCommand sqlcmd = new SqlCommand
        //        {
        //            Connection = SqlRepo.GetConnection(),
        //            CommandText = "SELECT containerid, albumname, lastimageindex FROM imagecontainers WHERE userid = @userID"
        //        };
        //        sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);

        //        SqlDataReader record = sqlcmd.ExecuteReader();
        //        while (record.Read())
        //        {
        //            albums.Add(new AlbumInfo(record["albumname"].ToString(), record["containerid"].ToString(), record["lastimageindex"].ToString()));
        //        }
        //    }

        //    return albums;
        //}

		public static List<AlbumInfo> GetAlbumsForUser(String userEmailId)
		{
			List<imagecontainer> imageContainerList = null;
			using (var context = new Entities())
			{
				imageContainerList = context.imagecontainers.Where(s => s.userid == userEmailId).ToList<imagecontainer>();
			}

			List<AlbumInfo> albums = new List<AlbumInfo>();
			foreach(imagecontainer imagecontainer in imageContainerList)
			{
				albums.Add(new AlbumInfo(imagecontainer.albumname, imagecontainer.containerid, imagecontainer.lastimageindex.ToString()));
			}
	
			return albums;
		}

		//public static AlbumInfo CreateAlbum(String UserEmailId, string AlbumName)
  //      {
  //          // TODO : These generation of random container name needs fix.
  //          // A new API needs to be created and put in utility file
  //          Random rnd = new Random();
  //          int number = rnd.Next(1, 100);
  //          string containername = "testcontainer" + number.ToString();
		//	string lastImageIndex = "0";
		//	AlbumInfo albumInfo = new AlbumInfo(AlbumName, containername, lastImageIndex);
  //          using (var SqlRepo = new SQLRepository())
  //          {
  //              SqlCommand sqlcmd = new SqlCommand
  //              {
  //                  Connection = SqlRepo.GetConnection(),
  //                  CommandText = "INSERT INTO imagecontainers VALUES (@containerName, @userID, @albumName, null, @lastImageIndex)"
  //              };
  //              sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);
  //              sqlcmd.Parameters.AddWithValue("@containerName", albumInfo.ContainerName);
  //              sqlcmd.Parameters.AddWithValue("@albumName", albumInfo.AlbumName);
		//		sqlcmd.Parameters.AddWithValue("@lastImageIndex", albumInfo.LastImageIndex);
  //              // TODO: Consider if we should catch primary key violation exception. Can 
  //              // two randomly generated containers names not be the same?
		//		sqlcmd.ExecuteNonQuery();
  //          }

		//	return albumInfo;
  //      }

		public static AlbumInfo CreateAlbum(String userEmailId, string albumName)
		{
			// TODO : These generation of random container name needs fix.
			// A new API needs to be created and put in utility file
			Random rnd = new Random();
			int number = rnd.Next(1, 100);
			string containername = "testcontainer" + number.ToString();
			string lastImageIndex = "0";
			AlbumInfo albumInfo = new AlbumInfo(albumName, containername, lastImageIndex);

			using (var context = new Entities())
			{
				var imageContainer = new imagecontainer
				{
					containerid = albumInfo.ContainerName,
					userid = userEmailId,
					albumname = albumInfo.AlbumName,
					albumid = null,
					lastimageindex = 0
				};

				context.imagecontainers.Add(imageContainer);
				context.SaveChanges();
			}

			return albumInfo;
		}

		//public static bool DeleteAlbum(string containerName)
  //      {
  //          // Delete entry from database
  //          using (var SqlRepo = new SQLRepository())
  //          {
  //              SqlCommand sqlcmd = new SqlCommand
  //              {
  //                  Connection = SqlRepo.GetConnection(),
  //                  CommandText = "DELETE FROM imagecontainers WHERE containerid = @containerName"
  //              };
  //              sqlcmd.Parameters.AddWithValue("@containerName", containerName);
  //              if (sqlcmd.ExecuteNonQuery() != 1)
  //              {
  //                  return false;
  //              }
  //          }
  //          // Delete from Blob store
  //          using (var ImageRepo = new ImageRepository(containerName))
  //          {
  //              var blobContainer = ImageRepo.GetCloudBlobContainer();
  //              blobContainer.DeleteIfExists();
  //          }
  //          return true;
  //      }

		public static bool DeleteAlbum(string containerName)
		{
			// Delete entry from database
			using (var context = new Entities())
			{
				imagecontainer imageContainer = context.imagecontainers.Where(s => s.containerid == containerName).FirstOrDefault<imagecontainer>();
				context.imagecontainers.Remove(imageContainer);
				context.SaveChanges();
			}

			// Delete from Blob store
			using (var ImageRepo = new ImageRepository(containerName))
			{
				var blobContainer = ImageRepo.GetCloudBlobContainer();
				blobContainer.DeleteIfExists();
			}

			return true;
		}

		//public static void UpdateLastImageIndex(string containerId, int lastImageIndex)
		//{
		//	using (var SqlRepo = new SQLRepository())
		//	{
		//		SqlCommand sqlcmd = new SqlCommand
		//		{
		//			Connection = SqlRepo.GetConnection(),
		//			CommandText = "UPDATE imagecontainers set lastimageindex = @lastImageIndex where containerid = @containerId"
		//		};
		//		sqlcmd.Parameters.AddWithValue("@lastImageIndex", lastImageIndex);
		//		sqlcmd.Parameters.AddWithValue("@containerId", containerId);
		//		sqlcmd.ExecuteNonQuery();
		//	}
		//}

		public static void UpdateLastImageIndex(string containerId, int lastImageIndex)
		{
			using (var context = new Entities())
			{
				var imageContainer = context.imagecontainers.Where(s => s.containerid == containerId).FirstOrDefault<imagecontainer>();
				if (imageContainer != null)
				{
					imageContainer.lastimageindex = lastImageIndex;
					context.SaveChanges();
				}
			}
		}
	}
}