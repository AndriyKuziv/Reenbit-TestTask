using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Communication.Email;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Reenbit_TestTask.FunctionApp.Entities;

namespace Reenbit_TestTask.FunctionApp
{
    [StorageAccount("BlobConnectionString")]
    public class UploadNotification
    {
        private readonly IConfiguration _configuration;
        private readonly BlobContainerClient _blobContainerClient;

        public UploadNotification(IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            _configuration = configuration;
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(_configuration.GetValue<string>("ContainerName"));
        }

        [FunctionName("UploadNotification")]
        public async Task Run([BlobTrigger("test-task-container/{name}")] Stream myBlob,
            string name,
            [DurableClient] IDurableOrchestrationClient orchestrationClient,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var prepEmailId = await orchestrationClient.StartNewAsync<string>("prepEmail", name);
            log.LogInformation("prepEmail id: " + prepEmailId);
        }

        [FunctionName("prepEmail")]
        public async Task prepEmail([OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var name = context.GetInput<string>();

            var entityId = new EntityId(nameof(RecipientEmail), "myParams");
            logger.LogInformation($"Current id: {entityId}; getting a value...");

            string currentValue = await context.CallEntityAsync<string>(entityId, "Get");
            logger.LogInformation("Done!");
            logger.LogInformation("currValue: " + currentValue);

            string? email = currentValue;
            logger.LogInformation($"{email}");
            if (string.IsNullOrEmpty(email))
            {
                logger.LogError("Email of a recipient does not exist");
                return;
            }

            SendEmail(email, CreateFileUri(name));
        }

        private void SendEmail(string recipientEmail, string uri)
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                Console.WriteLine("Email is null");
                return;
            }

            string connectionString = _configuration.GetValue<string>("AzureCommResource");
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

            var emailMessage = new EmailMessage(_configuration.GetValue<string>("EmailDomainName"), emailRecipients, emailContent);

            try
            {
                var emailSendOperation = emailClient.Send(WaitUntil.Completed, emailMessage);
                Console.WriteLine($"Email Sent. Status = {emailSendOperation.Value.Status}");

                var operationId = emailSendOperation.Id;
                Console.WriteLine($"Email operation id = {operationId}");
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
            }
        }

        private string CreateFileUri(string docName, string storedPolicyName = null)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _configuration.GetValue<string>("ContainerName"),
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

            string docUri = _blobContainerClient.Uri.AbsoluteUri + '/' + docName;

            return sasURI.AbsoluteUri;
        }
    }
}