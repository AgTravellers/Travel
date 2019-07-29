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

		[Authorize]
		[HttpPost]
		public ActionResult Upload1()
		{
			if (Request.Files.Count > 0)
			{
				try
				{
					string containerName = Request.Params["containername"];
					int lastImageIndex = int.Parse(Request.Params["lastimageindex"]);
					 
					//  Get all files from Request object  
					HttpFileCollectionBase files = Request.Files;
					List<string> savedFilePaths = new List<string>();
					for (int i = 0; i < files.Count; i++)
					{
						//string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
						//string filename = Path.GetFileName(Request.Files[i].FileName);  

						HttpPostedFileBase file = files[i];
						string fname;

						// Checking for Internet Explorer  
						if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
						{
							string[] testfiles = file.FileName.Split(new char[] { '\\' });
							fname = string.Format("{0}_{1}", lastImageIndex, testfiles[testfiles.Length - 1]);
						}
						else
						{
							fname = string.Format("{0}_{1}", lastImageIndex, file.FileName);
						}

						// Get the complete folder path and store the file inside it.
						fname = Path.Combine(Server.MapPath("~/LocalStore/Uploads/"), fname);
						file.SaveAs(fname);
						savedFilePaths.Add(fname);
						lastImageIndex++;
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

					// Update the lastimageIndex of the container
					AlbumManagement.UpdateLastImageIndex(containerName, lastImageIndex);

					// Returns message that successfully uploaded  
					return Json("File Uploaded Successfully!");
				}
				catch (Exception ex)
				{
					return Json("Error occurred. Error details: " + ex.Message);
				}
			}
			else
			{
				return Json("No files selected.");
			}
		}
			
	}
}