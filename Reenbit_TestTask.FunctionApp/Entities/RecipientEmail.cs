using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Text;
using System.Threading.Tasks;
using DurableTask.Core.Stats;
using Microsoft.Azure.WebJobs;

namespace Reenbit_TestTask.FunctionApp.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RecipientEmail
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        public Task Set(string email)
        {
            Email = email;
            return Task.CompletedTask;
        }

        public Task<string> Get()
        {
            return Task.FromResult(Email);
        }

        [FunctionName(nameof(RecipientEmail))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<RecipientEmail>();
    }
}
