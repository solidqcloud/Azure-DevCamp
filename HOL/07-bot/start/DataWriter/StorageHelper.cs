using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure;

namespace DataWriter
{
    class StorageHelper
    {
        private static string AZURE_STORAGE_ACCOUNT = CloudConfigurationManager.GetSetting("AZURE_STORAGE_ACCOUNT");
        private static string AZURE_STORAGE_ACCESS_KEY = CloudConfigurationManager.GetSetting("AZURE_STORAGE_ACCESS_KEY");
        private static string AZURE_STORAGE_BLOB_CONTAINER = CloudConfigurationManager.GetSetting("AZURE_STORAGE_BLOB_CONTAINER");
        private static string AZURE_STORAGE_QUEUE = CloudConfigurationManager.GetSetting("AZURE_STORAGE_QUEUE");
        private static string AZURE_STORAGE_CONNECTIONSTRING = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", AZURE_STORAGE_ACCOUNT, AZURE_STORAGE_ACCESS_KEY);

        /// <summary>
        /// Adds an incident message to the queue
        /// </summary>
        /// <param name="IncidentId">The incident ID from the service</param>
        /// <param name="ImageFileName">The file name of the image</param>
        /// <returns></returns>
        public static async Task AddMessageToQueue(string IncidentId, string ImageFileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AZURE_STORAGE_CONNECTIONSTRING);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue msgQ = queueClient.GetQueueReference(AZURE_STORAGE_QUEUE);
            msgQ.CreateIfNotExists();

            JObject qMsgJson = new JObject();
            qMsgJson.Add("IncidentId", IncidentId);
            qMsgJson.Add("BlobContainerName", AZURE_STORAGE_BLOB_CONTAINER);
            qMsgJson.Add("BlobName", getIncidentBlobFilename(IncidentId, ImageFileName));

            var qMsgPayload = JsonConvert.SerializeObject(qMsgJson);
            CloudQueueMessage qMsg = new CloudQueueMessage(qMsgPayload);

            await msgQ.AddMessageAsync(qMsg);
        }

        /// <summary>
        /// Uploads a blob to the configured storage account
        /// </summary>
        /// <param name="IncidentId">The IncidentId the image is associated with</param>
        /// <param name="imageFile">The File</param>
        /// <returns>The Url to the blob</returns>
        public static async Task<string> UploadFileToBlobStorage(string IncidentId, Stream imageFile, String imageType, String imageName)
        {
            string imgUri = string.Empty;

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AZURE_STORAGE_CONNECTIONSTRING);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(AZURE_STORAGE_BLOB_CONTAINER);
                container.CreateIfNotExists();
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                CloudBlockBlob imgBlob = container.GetBlockBlobReference(getIncidentBlobFilename(IncidentId, imageName));
                imgBlob.Properties.ContentType = imageType;
                await imgBlob.UploadFromStreamAsync(imageFile);

                var uriBuilder = new UriBuilder(imgBlob.Uri);
                uriBuilder.Scheme = "https";
                imgUri = uriBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new HttpUnhandledException($"Unable to upload image for incident {IncidentId} to blob storage. Error:: ${ex.ToString()}");
            }
            return imgUri;
        }

        private static string getIncidentBlobFilename(string IncidentId, string FileName)
        {
            string fileExt = Path.GetExtension(FileName);
            //Remove the starting . if exists
            if (fileExt.StartsWith("."))
            {
                fileExt.TrimStart(new char[] { '.' });
            }
            return $"{IncidentId}{fileExt}";
        }
    }
}
