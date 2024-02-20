using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;

namespace Reenbit_TestTask.Server.Tests.Unit
{
    public class DocumentsControllerTests
    {
        private readonly Mock<IDocumentsRepository> _documentsRepositoryMock = new Mock<IDocumentsRepository>();
        private Mock<IFormFile> _formFileMock = new Mock<IFormFile>();
        private readonly string _formFileMockName = "test.docx";
        private readonly string _email = "email@test.com";
        private readonly UploadDocRequest _uploadDocRequest;

        public DocumentsControllerTests()
        {
            _uploadDocRequest = new UploadDocRequest()
            {
                File = _formFileMock.Object,
                Email = _email
            };
        }

        [Fact]
        public async Task UploadDoc_ReturnsOkResult_WithUri()
        {
            // Arrange
            var expectedUri = "https://test.com/testDoc.docx";
            _documentsRepositoryMock.Setup(repo => repo.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                                                      .ReturnsAsync(expectedUri);
            var controller = new DocumentsController(_documentsRepositoryMock.Object);

            // Act
            var response = await controller.UploadDoc(_uploadDocRequest);

            // Assert
            Assert.NotNull(response);
            var result = Assert.IsType<OkObjectResult>(response);
            var responseUri = Assert.IsType<DocUri>(result.Value);
            Assert.Equal(expectedUri, responseUri.Uri);
        }
    }
}