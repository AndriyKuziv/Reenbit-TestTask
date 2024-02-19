using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Reenbit_TestTask.Server.Repositories
{
    public interface IDocumentsRepository
    {

        Task<string> UploadAsync(IFormFile docFile, string docName);
    }
}