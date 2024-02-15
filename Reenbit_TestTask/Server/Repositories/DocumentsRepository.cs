﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Reenbit_TestTask.Server.Extensions;
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

        public async Task<string> UploadAsync(IFormFile docFile)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            string docName = GenerateDocName();

            using Stream data = docFile.OpenReadStream();
            await _blobContainerClient.UploadBlobAsync(docName, data);

            string docUri = CreateDocUri(docName);

            return docUri;
        }

        private string GenerateDocName()
        {
            string docName = Guid.NewGuid().ToString() + ".docx";

            return docName;
        }

        private string CreateDocUri(string docName)
        {
            string docUri = _blobContainerClient.Uri.AbsoluteUri + '/' + docName;

            return docUri;
        }
    }
}