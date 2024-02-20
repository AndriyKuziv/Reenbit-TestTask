using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using Reenbit_TestTask.Shared.Models.DTO;

namespace Reenbit_TestTask.Client.Pages
{
    public class IndexComponent : ComponentBase
    {
        public UploadModel? Model { get; set; }

        [Parameter]
        public LinkStyle Attributes { get; set; }

        protected override void OnInitialized()
        {
            Model ??= new();
            Attributes = new LinkStyle()
            {
                Display = "none",
                TextContent = "Loading..."
            };
        }

        public class UploadModel
        {
            [Required(ErrorMessage = "Please provide an email")]
            [EmailAddress(ErrorMessage = "Please provide a valid email")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Please provide a file")]
            public IBrowserFile? File { get; set; }
        }

        public class LinkStyle
        {
            public string Display { get; set; }
            public string TextContent { get; set; }
        }

        public void ChangeFile(InputFileChangeEventArgs e)
        {
            if (Model != null)
            {
                Model.File = e.File;
            }
        }

        public async Task Submit()
        {
            if (Model is null || Model.File is null) return;

            Attributes.Display = "block";
            Attributes.TextContent = "Loading...";

            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(Model.File.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(Model.File.ContentType);
            content.Add(
                content: fileContent,
                name: "\"file\"",
                fileName: Model.File.Name
            );

            var emailContent = new StringContent(Model.Email);
            content.Add(
                content: emailContent,
                name: "\"email\""
            );

            var client = new HttpClient();

            string callPath = "https://reenbittesttaskappservice.azurewebsites.net/documents/upload";
            var response = await client.PostAsync(callPath, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponseContent = JsonSerializer.Deserialize<UploadDocResponse>(responseContent);

            SuccessNotification();
        }

        private void SuccessNotification()
        {
            Attributes.Display = "block";
            Attributes.TextContent = "Document has been uploaded successfully! Please wait for the email notification";
        }
    }
}
