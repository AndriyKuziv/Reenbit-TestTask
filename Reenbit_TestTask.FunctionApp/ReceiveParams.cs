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
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Reenbit_TestTask.FunctionApp.Entities;
using Newtonsoft.Json.Linq;

namespace Reenbit_TestTask.FunctionApp
{
    public static class ReceiveParams
    {
        private readonly static string _paramsFile = "tempParams.json";

        [FunctionName("ReceiveParams")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string email = data?.email;

            string responseMessage = string.Empty;

            if(string.IsNullOrEmpty(email))
            {
                return new BadRequestResult();
            }

            //var entityId = new EntityId(nameof(RecipientEmail), "myParams");
            //await client.SignalEntityAsync(entityId, "Set", email);

            EmailRequestParams content;
            if (File.Exists(_paramsFile))
            {
                var jsonContent = File.ReadAllText(_paramsFile);
                content = JsonConvert.DeserializeObject<EmailRequestParams>(jsonContent);
                content.Email = email;
            }
            else
            {
                content = new EmailRequestParams()
                {
                    Email = email
                };
            }

            string updatedJsonContent = JsonConvert.SerializeObject(content);
            await File.WriteAllTextAsync(_paramsFile, updatedJsonContent);

            log.LogInformation("Set email: " + content.Email);

            responseMessage = $"Hello, {email}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
