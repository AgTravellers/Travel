using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelApp.WebAPIs;
using TravelApp.DataModels;
using System.Web.Http;

namespace TravelApp.Controllers
{
    public class AlbumsController : Controller
    {

        [System.Web.Mvc.Authorize]
        public ActionResult Albums()
        {
            string UserEmailId = User.Identity.GetUserName();
            UserProfile UserProf = UserManagement.GetUserProfile(UserEmailId);

            if (String.IsNullOrWhiteSpace(UserProf.UserId))
            {
                // First login, so we need to create an entry
                // in userprofile table
                UserManagement.CreateUserProfile(UserEmailId);
            }

            return View();
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult GetAlbums()
        {
            // Can be enhanced to a list of album infos
            List<AlbumInfo> Albums = new List<AlbumInfo>();
            string UserEmailId = User.Identity.GetUserName();
            Albums = AlbumManagement.GetAlbumsForUser(UserEmailId);
            return Json(Albums, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public void CreateAlbum([FromBody] string name)
        {
            string UserEmailId = User.Identity.GetUserName();
            AlbumManagement.CreateAlbum(UserEmailId, name);
        }
    }
}