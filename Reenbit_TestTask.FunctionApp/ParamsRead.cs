using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Reenbit_TestTask.FunctionApp.Entities;

namespace Reenbit_TestTask.FunctionApp
{
    public static class ParamsRead
    {
        [FunctionName("ReadParams")]
        public static async Task<string> ReadParams(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var entityId = new EntityId(nameof(RecipientEmail), "myParams");
            logger.LogInformation("Reading entity value...");
            string currentValue = await context.CallEntityAsync<string>(entityId, "Get");
            logger.LogInformation("Done!");
            logger.LogInformation("currValue: " + currentValue);

            return currentValue;
        }
    }
}