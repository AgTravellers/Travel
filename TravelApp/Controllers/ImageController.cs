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
using TravelApp.DataModels;
using TravelApp.WebAPIs;
using TravelApp.Utils;

namespace TravelApp.Controllers
{
    public class ImageController : Controller
    {
        [Authorize]
        public ActionResult Images()
        {
            ViewBag.imageUrls = new List<string>();
            string UserEmailId = User.Identity.GetUserName();
            UserProfile UserProf = UserManagement.GetUserProfile(UserEmailId);

            if (String.IsNullOrWhiteSpace(UserProf.UserId))
            {
                // First login, so we need to create an entry
                // in userprofile table
                UserManagement.CreateUserProfile(UserEmailId);
            }

            if (String.IsNullOrWhiteSpace(UserProf.ImageContainerId))
            {
                // No container present, so no images
                ViewBag.message = "You do not have any pictures yet. Start adding them";
            }
            else
            {
                // List of image URIs fetched
                ViewBag.imageUrls = ImageManagement.GetImageUrisFromContainer(UserProf.ImageContainerId);
            }

            return View();
        }

		//[Authorize]
		//public ActionResult Upload(string imageUrl)
		//{
		//	if (imageUrl == null)
		//	{
		//		Console.WriteLine("Failed to get image Url");
		//	}

		//	ViewBag.message = "Image successfully updated";
		//	return View("~/Views/Image/Images.cshtml");
		//}

		[Authorize]
		[HttpPost]
		public ActionResult Upload(List<HttpPostedFileBase> postedFiles)
		{
			// First save all the files in local disk
			string path = Server.MapPath("~/LocalStore/Uploads/");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			List<string> savedFilePaths = new List<string>();
			string failedFileNames = String.Empty;
			foreach (HttpPostedFileBase postedFile in postedFiles)
			{
				if (postedFile != null)
				{
					string fileName = Path.GetFileName(postedFile.FileName);
					if (Utility.FileSizeInMegabytes(postedFile) <= 4)
					{
						postedFile.SaveAs(path + fileName);
						savedFilePaths.Add(path + fileName);
					}
					else
					{
						failedFileNames += String.IsNullOrWhiteSpace(failedFileNames) ? fileName : string.Format(", {0}", fileName);
					}
				}
			}

			if (String.IsNullOrWhiteSpace(failedFileNames))
			{
				ViewBag.UploadFailureMessage = string.Format("Failed to upload {0}. Currently images with size greater than 4MB is not supported", failedFileNames);
			}

			// Upload to blob storage
			string userId = User.Identity.GetUserName();
			string imageContainerId = ImageManagement.GetOrCreateImageContainerIdForUser(userId);

			using (var ImageRepo = new ImageRepository(imageContainerId))
			{
				var blobContainer = ImageRepo.GetCloudBlobContainer();

				foreach (string filepath in savedFilePaths)
				{
					CloudBlockBlob cloudBlockBlob = blobContainer.GetBlockBlobReference(imageContainerId);
					cloudBlockBlob.UploadFromFile(filepath);
					ViewBag.UploadMessage += string.Format("<b>{0}</b> uploaded.<br />", cloudBlockBlob.Uri.ToString());
				}
			}

			return View("~/Views/Image/Images.cshtml");
		}

        // No used, kept just for reference
        // ToDo : Remove
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

        // No used, kept just for reference
        // ToDo : Remove
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