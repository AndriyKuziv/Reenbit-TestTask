using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Reenbit_TestTask.Shared.Models.DTO
{
    public class DocUri
    {
        [JsonPropertyName("uri")]
        public required string Uri { get; set; }
    }
}
