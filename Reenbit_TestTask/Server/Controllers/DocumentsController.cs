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
            var uri = await _documentsRepository.UploadAsync(uploadDocRequest.File, GenerateDocName());

            var uriDTO = new DocUri()
            {
                Uri = uri,
            };

            return Ok(uriDTO);
        }

        private async Task SetEmail(UploadDocRequest uploadDocRequest)
        {
            string funcUrl = "https://reenbittesttaskfunctionapp.azurewebsites.net/api/ReceiveParams?code=2IR-gZQuM9O_DuRw_kG0rJN86Q8c43fRJjaMIavTHe25AzFufpBQFg==";

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