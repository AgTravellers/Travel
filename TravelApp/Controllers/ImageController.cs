using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using TravelApp.Storage.ImageStore;
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

		[Authorize]
		[HttpPost]
		public ActionResult Upload()
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

					// Now the images are uploaded to blob, delete them from local store.
					// TODO : Revisit the delete logic when if we need implement disk cache. (really do we need to ?)
					foreach (string filepath in savedFilePaths)
					{
						if (System.IO.File.Exists(filepath))
						{
							System.IO.File.Delete(filepath);
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

		[Authorize]
		[HttpGet]
		public ActionResult Delete(string containerName, string filepath)
		{
			if (String.IsNullOrWhiteSpace(containerName) || String.IsNullOrWhiteSpace(filepath))
			{
				return Json("Failed to delete file", JsonRequestBehavior.AllowGet);
			}

			using (var ImageRepo = new ImageRepository(containerName))
			{
				string fileName = Path.GetFileName(filepath);
				var blobContainer = ImageRepo.GetCloudBlobContainer();
				CloudBlockBlob cloudBlockBlob = blobContainer.GetBlockBlobReference(fileName);
				cloudBlockBlob.DeleteIfExists();
			}

			return Json("Successfully deleted file", JsonRequestBehavior.AllowGet);
		}

	}
}