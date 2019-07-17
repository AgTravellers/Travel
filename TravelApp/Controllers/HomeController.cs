using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelApp.Storage.SQL;
using TravelApp.Storage.ImageStore;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace TravelApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            string message = "Not Connected";
            using (var SqlRepo = new SQLRepository())
            {
                string query = "select message from testtable";

                SqlCommand oCmd = new SqlCommand(query, SqlRepo.GetConnection());
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        message = oReader["message"].ToString();
                    }
                    //message = "Connected to database";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {

            ViewBag.Message = "This is contacts page for " + User.Identity.GetUserName();
            return View();
        }


    }
}