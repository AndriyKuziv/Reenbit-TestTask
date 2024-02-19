using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit_TestTask.FunctionApp.Models
{
    public class EmailRequestParams
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}
