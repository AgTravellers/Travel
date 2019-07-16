using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using TravelApp.Storage.ImageStore;
using TravelApp.Storage.SQL;
//using TravelApp.DataManagers;

namespace TravelApp.Controllers
{
    public class ImageController : Controller
    {
        public ActionResult Images()
        {
            //ImageManager iManager = new ImageManager();
            //string imageName = iManager.GetImageName(User.Identity.) 
            string ImageContainer = String.Empty;
            //using (var SqlRepo = new SQLRepository("Data Source=SOGHO-LAPTOP;Initial Catalog=travelappdb;User ID=sa;Password='hujugshuruholo2019$'"))
            using (var SqlRepo = new SQLRepository("Server=tcp:ta-sqlserver.database.windows.net,1433;Initial Catalog=ta-sqldb;Persist Security Info=False;User ID=ta-sqluser;Password=hujugshuruholo2019$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                string query = "select container_name from userimagecontainers";
                SqlDataReader oReader = SqlRepo.GetRecords(query);
                oReader.Read();
                try
                {
                    ImageContainer = oReader["container_name"].ToString();
                }
                catch
                {

                }
            }
            if (String.IsNullOrEmpty(ImageContainer))
            {
                ViewBag.message = "You do not have any pictures yet. Start adding them";
                ViewBag.image = "";
            }
            else
            {
                string imageName = string.Empty;
                using (var ImageRepo = new ImageRepository(ImageContainer))
                {
                    var container = ImageRepo.GetCloudBlobContainer();
                    container.CreateIfNotExistsAsync().Wait();
                    // Set the permissions so the blobs are public.
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    container.SetPermissionsAsync(permissions).Wait();
                    string imageUri = UploadToBlobStorage(container);
                    imageName = GetImageFromBlobStorage(imageUri);
                }
                ViewBag.image = @"~/Content/Images/" + imageName;
            }

            return View();
        }

        private static string UploadToBlobStorage(CloudBlobContainer container)
        {
            string localPath = @"C:\\Users\\sogho\\Desktop";
            string localFileName = "TestImage1.jpg";
            var sourceFile = Path.Combine(localPath, localFileName);

            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(localFileName);
            cloudBlockBlob.UploadFromFile(sourceFile);
            return cloudBlockBlob.Uri.ToString();
        }

        private static string GetImageFromBlobStorage(String imageUri)
        {

            CloudBlockBlob cloudBlockBlob = new CloudBlockBlob(new System.Uri(imageUri));
            string localPath = @"C:\Users\sogho\Documents\GitHub\Travel\TravelApp\Content\Images";
            string localFileName = "TestImage1_downloaded.jpg";
            var destinationFile = Path.Combine(localPath, localFileName);
            cloudBlockBlob.DownloadToFile(destinationFile, FileMode.Create);
            return localFileName;
        }
    }
}