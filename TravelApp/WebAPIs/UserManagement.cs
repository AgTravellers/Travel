using System;
using System.Linq;
using TravelApp.DataModels;

namespace TravelApp.WebAPIs
{
	public class UserManagement
	{
		public static UserProfile GetUserProfile(string userEmailId)
		{
			userprofile userProfile = null;
			using (var context = new Entities())
			{
				userProfile = context.userprofiles.Where(s => s.userid == userEmailId).FirstOrDefault<userprofile>();
			}

			return new UserProfile(userProfile.userid, userProfile.imagecontainerid);
		}

		//public static void CreateUserProfile(string UserEmailId)
		//{
		//	using (var SqlRepo = new SQLRepository())
		//	{
		//		SqlCommand sqlcmd = new SqlCommand
		//		{
		//			Connection = SqlRepo.GetConnection(),
		//			CommandText = @"INSERT INTO userprofile VALUES (@userID, null)"
		//		};
		//		sqlcmd.Parameters.AddWithValue("@userID", UserEmailId);
		//		sqlcmd.ExecuteNonQuery();
		//	}
		//}

		public static void CreateUserProfile(string userEmailId)
		{
			using (var context = new Entities())
			{
				var std = new userprofile()
				{
					userid = userEmailId,
					imagecontainerid = null
				};

				context.userprofiles.Add(std);
				context.SaveChanges();
			}
		}

		//public static string CreateImageContainerForUser(string userid)
		//{
		//	// get a unique container guid
		//	Guid imageContainerGuid = Guid.NewGuid();
		//	string containerId = "testcontainertwo";// imageContainerGuid.ToString();

		//	using (var SqlRepo = new SQLRepository())
		//	{
		//		SqlCommand sqlcmd = new SqlCommand
		//		{
		//			Connection = SqlRepo.GetConnection(),
		//			CommandText = @"update userprofile set imagecontainerid = @ContainerId where userid = @userid"
		//		};

		//		sqlcmd.Parameters.AddWithValue("@userID", userid);
		//		sqlcmd.Parameters.AddWithValue("@ContainerId", containerId);
		//		sqlcmd.ExecuteNonQuery();
		//	}

		//	// TODO : How do we detect query execution failures ?
		//	return containerId;
		//}

		public static string CreateImageContainerForUser(string userid)
		{
			// Get a unique container guid
			Guid imageContainerGuid = Guid.NewGuid();
			// TODO : Replace the logic for unique container id
			string containerId = "testcontainertwo";// imageContainerGuid.ToString();

			userprofile userProfile = null;
			using (var context = new Entities())
			{
				userProfile = context.userprofiles.Where(s => s.userid == userid).FirstOrDefault<userprofile>();
				userProfile.imagecontainerid = containerId;
				context.SaveChanges();
			}

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