using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit_TestTask.Shared.Models.DTO
{
    public class UploadDocRequest
    {
        public IFormFile File {  get; set; }

        public string Email { get; set; }
    }
}
