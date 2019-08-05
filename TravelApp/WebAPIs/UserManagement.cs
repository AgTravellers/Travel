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

			if (userProfile != null)
			{
				return new UserProfile(userProfile.userid, userProfile.imagecontainerid);
			}

			return null;
		}

		public static void CreateUserProfile(string userEmailId)
		{
			using (var context = new Entities())
			{
				var userProfile = new userprofile()
				{
					userid = userEmailId,
					imagecontainerid = null
				};

				context.userprofiles.Add(userProfile);
				context.SaveChanges();
			}
		}
	}
}