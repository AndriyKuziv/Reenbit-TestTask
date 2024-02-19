using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Reenbit_TestTask.Server.Tests.Unit
{
    public class DocumentsRepositoryTests
    {
        private DocumentsRepository _documentsRepository;
        private readonly Mock<BlobServiceClient> _blobServiceClientMock = new Mock<BlobServiceClient>();
        private readonly Mock<BlobContainerClient> _blobContainerClientMock = new Mock<BlobContainerClient>();
        private readonly Mock<IFormFile> _formFileMock = new Mock<IFormFile>();
        private readonly string _formFileMockName = "test.docx";


        public DocumentsRepositoryTests()
        {
            _documentsRepository = new DocumentsRepository(_blobServiceClientMock.Object);
        }

        public async Task UploadAsync_ReturnsUri()
        {
            // Arrange
            var expectedUri = "https://test.com/testDoc.docx";

            // Act
            var resultUri = await _documentsRepository.UploadAsync(_formFileMock.Object, _formFileMockName);

            // Assert
            Assert.Equal(expectedUri, resultUri);
        }
    }
}
