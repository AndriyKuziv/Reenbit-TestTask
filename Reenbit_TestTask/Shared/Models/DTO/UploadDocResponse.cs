using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Reenbit_TestTask.Shared.Models.DTO
{
    public class UploadDocResponse
    {
        [JsonPropertyName("docName")]
        public required string DocName { get; set; }
    }
}
