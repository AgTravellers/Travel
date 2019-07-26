using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApp.Storage.ImageStore
{
    public class ImageRepository : IDisposable
    {
        //private readonly string BlobStoreConnectionString = "UseDevelopmentStorage=true";
        private readonly string BlobStoreConnectionString = "DefaultEndpointsProtocol=https;AccountName=tastorageaccnt;AccountKey=bkWi1jbWLDJjMNn8FrLsI8I3cItllX1xH2RE9V0lc2CFEfbPZsvFf9iVE82hcJCOowRL9UUkg60cdrsxSAMz/Q==;EndpointSuffix=core.windows.net";
        private CloudBlobContainer CBlobContainer;
        public ImageRepository(string containerName)
        {
            // Parse the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(BlobStoreConnectionString);
            //Create the service client object for credentialed access to the Blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
			// Retrieve a reference to a container.
			
			CBlobContainer = blobClient.GetContainerReference(containerName);
			CBlobContainer.CreateIfNotExists();
			var permissions = CBlobContainer.GetPermissions();
			permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
			CBlobContainer.SetPermissions(permissions);
		}

        public CloudBlobContainer GetCloudBlobContainer()
        {
            return CBlobContainer;
        }

        public void Dispose()
        {
        }
    }
}