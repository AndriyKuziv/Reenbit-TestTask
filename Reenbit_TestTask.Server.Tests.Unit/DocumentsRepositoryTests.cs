using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Reenbit_TestTask.Server.Tests.Unit
{
    public class DocumentsRepositoryTests
    {
        private readonly Mock<BlobServiceClient> _blobServiceClientMock = new Mock<BlobServiceClient>();
        private readonly Mock<BlobContainerClient> _blobContainerClientMock = new Mock<BlobContainerClient>();
        private readonly Mock<BlobClient> _blobClientMock = new Mock<BlobClient>();
        private readonly Mock<IFormFile> _formFileMock = new Mock<IFormFile>();

        [Fact]
        public async Task UploadAsync_ReturnsUri()
        {
            // Arrange
            var expectedDocName = "newTestDocName.docx";

            _blobContainerClientMock.Setup(cclient => cclient.UploadBlobAsync(It.IsAny<string>(),
                It.IsAny<Stream>(), It.IsAny<CancellationToken>()));
            _blobContainerClientMock.Setup(cclient => cclient.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);

            _blobServiceClientMock.Setup(sclient => sclient.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_blobContainerClientMock.Object);

            var documentsRepository = new DocumentsRepository(_blobServiceClientMock.Object);

            // Act
            var resultDocName = await documentsRepository.UploadAsync(_formFileMock.Object, expectedDocName);

            // Assert
            Assert.NotNull(resultDocName);
            Assert.Equal(expectedDocName, resultDocName);
        }
    }
}
