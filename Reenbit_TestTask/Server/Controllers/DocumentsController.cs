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

        [HttpGet]
        public async Task<IActionResult> GetAllDocs()
        {
            return Ok();
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadDoc(IFormFile file)
        {
            var uri = await _documentsRepository.UploadAsync(file);

            var uriDTO = new DocUri()
            {
                Uri = uri,
            };

            return Ok(uriDTO);
        }
    }
}