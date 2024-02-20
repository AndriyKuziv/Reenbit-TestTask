using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Reenbit_TestTask.Server.Repositories;
using Reenbit_TestTask.Shared.Models.DTO;

namespace Reenbit_TestTask.Server.Controllers
{
    [ApiController]
    [Route("documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsRepository _documentsRepository;

        public DocumentsController(IDocumentsRepository documentsRepository)
        {
            _documentsRepository = documentsRepository;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadDoc([FromForm] UploadDocRequest uploadDocRequest)
        {
            await SetEmail(uploadDocRequest);
            var newDocName = await _documentsRepository.UploadAsync(uploadDocRequest.File, GenerateDocName());

            var docDTO = new UploadDocResponse()
            {
                DocName = newDocName,
            };

            return Ok(docDTO);
        }

        private async Task SetEmail(UploadDocRequest uploadDocRequest)
        {
            string funcUrl = "https://reenbittesttaskfunctionapp.azurewebsites.net/api/ReceiveParams?code=Vxk-a0s9O3cuaAzDiYtoPh3-nY7FHewj-oCBDZj_qgoAAzFuF5YeTw==";

            using HttpClient client = new HttpClient();
            var response = await client.PostAsync(funcUrl, 
                new StringContent("{\"email\": " + $"\"{uploadDocRequest.Email}\"" + "}"));
            Console.WriteLine(response.StatusCode);
            var httpContent = response.Content;

            string data = await httpContent.ReadAsStringAsync();
        }

        private string GenerateDocName()
        {
            string docName = Guid.NewGuid().ToString() + ".docx";

            return docName;
        }
    }
}