using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reenbit_TestTask.FunctionApp.Models;
using Azure.Storage.Blobs;
using System.Text;

namespace Reenbit_TestTask.FunctionApp
{
    public static class ReceiveParams
    {
        private readonly static string _paramsFile = "tempParams.json";
        private readonly static string _paramsContainer = "private-values";

        [FunctionName("ReceiveParams")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string email = data?.email;

            if(string.IsNullOrEmpty(email))
            {
                return new BadRequestResult();
            }

            try
            {
                await WriteFile(email, log);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);
            }

            string responseMessage = $"Hello, {email}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        private static async Task WriteFile(string email, ILogger log)
        {
            log.LogInformation("Getting a connString");
            string blobConnection = Environment.GetEnvironmentVariable("BlobConnectionString");
            log.LogInformation("Creating a BlobServiceClient");
            var blobServiceClient = new BlobServiceClient(blobConnection);
            log.LogInformation("Creating a BlobContainerClient");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_paramsContainer);
            log.LogInformation("Getting a blobClient");
            var blobClient = containerClient.GetBlobClient(_paramsFile);

            EmailRequestParams content = new EmailRequestParams()
            {
                Email = email
            };
            string updatedJsonContent = JsonConvert.SerializeObject(content);
            byte[] output = Encoding.UTF8.GetBytes(updatedJsonContent);

            log.LogInformation("Opening file stream...");
            using var fileStream = await blobClient.OpenWriteAsync(true);

            log.LogInformation("Writing data to file...");
            await fileStream.WriteAsync(output, 0, output.Length);

            await fileStream.FlushAsync();
            fileStream.Close();

            log.LogInformation("Done!");
        }
    }
}
