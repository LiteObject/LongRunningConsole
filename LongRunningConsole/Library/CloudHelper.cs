using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace LongRunningConsole.Library
{
    public class CloudHelper
    {
        /// <summary>
        /// The read blob async.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="containerName">
        /// The container name.
        /// </param>
        /// <param name="blobName">
        /// The blob name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<MemoryStream> ReadBlobAsync(string connectionString, string containerName, string blobName)
        {
            var memoryStream = new MemoryStream();

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(connectionString, out var cloudStorageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container.
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

                // Retrieve reference to a blob.
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

                var exists = cloudBlockBlob.ExistsAsync().Result;

                if (exists)
                {
                    await cloudBlockBlob.DownloadToStreamAsync(memoryStream).ConfigureAwait(false);
                }
                else
                {
                    Console.WriteLine($"Unable to retrieve \"{blobName}\"");
                }
            }
            else
            {
                Console.WriteLine("Unable to parse storage connection string.");
            }

            return memoryStream;
        }
    }
}
