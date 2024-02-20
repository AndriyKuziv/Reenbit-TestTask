using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Communication.Email;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Reenbit_TestTask.FunctionApp.Models;
using Newtonsoft.Json;

namespace Reenbit_TestTask.FunctionApp
{
    [StorageAccount("BlobConnectionString")]
    public class UploadNotification
    {
        private readonly string _paramsFile = "tempParams.json";
        private readonly static string _paramsContainer = "private-values";

        private readonly BlobContainerClient _blobContainerClient;

        public UploadNotification(BlobServiceClient blobServiceClient)
        {
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("MainContainerName"));
        }

        [FunctionName("UploadNotification")]
        public async Task Run([BlobTrigger("test-task-container/{name}", Connection = "BlobConnectionString")] Stream myBlob,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            try
            {
                log.LogInformation("Getting params from a file...");
                var requestParams = JsonConvert.DeserializeObject<EmailRequestParams>(await GetJsonParams());

                var email = requestParams.Email;
                log.LogInformation("Done!");

                log.LogInformation($"Email: {email}");
                if (string.IsNullOrEmpty(email))
                {
                    log.LogError("Email of a recipient does not exist");
                    return;
                }

                SendEmail(email, CreateFileUri(name));
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);
            }
        }

        private async Task<string> GetJsonParams()
        {
            string blobConnection = Environment.GetEnvironmentVariable("BlobConnectionString");
            var blobServiceClient = new BlobServiceClient(blobConnection);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_paramsContainer);
            var blobClient = blobContainerClient.GetBlobClient(_paramsFile);

            var response = await blobClient.DownloadAsync();

            using var reader = new StreamReader(response.Value.Content);

            return await reader.ReadToEndAsync();
        }

        private void SendEmail(string recipientEmail, string uri)
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                Console.WriteLine("Email is null");
                return;
            }

            string connectionString = Environment.GetEnvironmentVariable("AzureCommResource");
            EmailClient emailClient = new EmailClient(connectionString);

            var emailContent = new EmailContent("Document Upload")
            {
                PlainText = "Your document has been uploaded successfully!\n" +
                "Here is the link to access it:\n" + uri,
            };

            var toRecipients = new List<EmailAddress>
            {
                new (recipientEmail)
            };

            var emailRecipients = new EmailRecipients(toRecipients);

            var emailMessage = new EmailMessage(Environment.GetEnvironmentVariable("EmailDomainName"), emailRecipients, emailContent);

            var emailSendOperation = emailClient.Send(WaitUntil.Completed, emailMessage);
            Console.WriteLine($"Email Sent. Status = {emailSendOperation.Value.Status}");
        }

        private string CreateFileUri(string docName, string storedPolicyName = null)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = Environment.GetEnvironmentVariable("MainContainerName"),
                BlobName = docName,
                Resource = "b"
            };
            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            BlobClient blobClient = _blobContainerClient.GetBlobClient(docName);

            Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

            return sasURI.AbsoluteUri;
        }
    }
}
