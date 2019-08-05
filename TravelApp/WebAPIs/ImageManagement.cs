using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using TravelApp.Storage.ImageStore;

namespace TravelApp.WebAPIs
{
	public class ImageManagement
	{
		public static List<string> GetImageUrisFromContainer(string ImageContainer)
		{
			List<string> images = new List<string>();
			using (var ImageRepo = new ImageRepository(ImageContainer))
			{
				var blobContainer = ImageRepo.GetCloudBlobContainer();
				BlobResultSegment blobResultSegment = null;
				do
				{
					blobResultSegment = blobContainer.ListBlobsSegmented(new BlobContinuationToken());
					foreach (var result in blobResultSegment.Results)
					{
						images.Add(result.Uri.ToString());
					}
				} while (blobResultSegment.ContinuationToken != null);
			}

			return images;
		}
	}
}