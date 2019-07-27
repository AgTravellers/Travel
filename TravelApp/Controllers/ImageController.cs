﻿using Microsoft.WindowsAzure.Storage.Blob;
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

namespace TravelApp.Controllers
{
    public class ImageController : Controller
    {
        [Authorize]
        public ActionResult Images(string id, string albumname, string containername)
        {
            ViewBag.imageUrls = new List<string>();

            if (String.IsNullOrWhiteSpace(albumname))
            {
                return Redirect("/Albums/Albums");
            }
            // Set the name of the album
            ViewBag.albumName = albumname;
            ViewBag.containerName = containername;
            ViewBag.message = "You do not have any images in this album. Start adding.";

            if (!String.IsNullOrWhiteSpace(containername))
            {
                // List of image URIs fetched
                ViewBag.imageUrls = ImageManagement.GetImageUrisFromContainer(containername);
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
		public ActionResult Upload(List<HttpPostedFileBase> postedFiles, string containerName)
		{
			// First save all the files in local disk
			string path = Server.MapPath("~/LocalStore/Uploads/");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			List<string> savedFilePaths = new List<string>();
			foreach (HttpPostedFileBase postedFile in postedFiles)
			{
				if (postedFile != null)
				{
					string fileName = Path.GetFileName(postedFile.FileName);
					postedFile.SaveAs(path + fileName);
					savedFilePaths.Add(path + fileName);
				}
			}

			// Upload to blob storage
			string userId = User.Identity.GetUserName();

            using (var ImageRepo = new ImageRepository(containerName))
            {
                var blobContainer = ImageRepo.GetCloudBlobContainer();

                foreach (string filepath in savedFilePaths)
                {
                    CloudBlockBlob cloudBlockBlob = blobContainer.GetBlockBlobReference(Path.GetFileName(filepath));
                    cloudBlockBlob.UploadFromFile(filepath);
                    ViewBag.UploadMessage += string.Format("<b>{0}</b> uploaded.<br />", cloudBlockBlob.Uri.ToString());
                }
            }

			return View("~/Views/Image/Images.cshtml");
		}

        // Not used, kept just for reference
        // ToDo : Remove
        //private static string UploadToBlobStorage(CloudBlobContainer container)
        //{
        //    string localPath = @"C:\\Users\\sogho\\Desktop";
        //    string localFileName = "TestImage1.jpg";
        //    var sourceFile = Path.Combine(localPath, localFileName);

        //    // Get a reference to the blob address, then upload the file to the blob.
        //    // Use the value of localFileName for the blob name.
        //    CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(localFileName);
        //    cloudBlockBlob.UploadFromFile(sourceFile);
        //    return cloudBlockBlob.Uri.ToString();
        //}

        // Not used, kept just for reference
        // ToDo : Remove
        //private static string GetImageFromBlobStorage(String imageUri)
        //{

        //    CloudBlockBlob cloudBlockBlob = new CloudBlockBlob(new System.Uri(imageUri));
        //    string localPath = @"C:\Users\sogho\Documents\GitHub\Travel\TravelApp\Content\Images";
        //    string localFileName = "TestImage1_downloaded.jpg";
        //    var destinationFile = Path.Combine(localPath, localFileName);
        //    cloudBlockBlob.DownloadToFile(destinationFile, FileMode.Create);
        //    return localFileName;
        //}
    }
}