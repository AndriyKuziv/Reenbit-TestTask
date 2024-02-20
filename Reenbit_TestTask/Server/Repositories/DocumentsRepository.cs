using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;

namespace Reenbit_TestTask.Server.Repositories
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private readonly BlobContainerClient _blobContainerClient;
        private const string containerName = "test-task-container";

        public DocumentsRepository(BlobServiceClient blobServiceClient)
        {
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> UploadAsync(IFormFile docFile, string docName)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            using Stream data = docFile.OpenReadStream();
            await _blobContainerClient.UploadBlobAsync(docName, data);

            return docName;
        }

    }
}