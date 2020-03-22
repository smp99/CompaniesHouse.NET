using System.IO;
using System.Threading.Tasks;
using CompaniesHouse.Response.Document;
using NUnit.Framework;

namespace CompaniesHouse.IntegrationTests.Tests.DocumentTests
{
    [TestFixture("Mw2JX3NUZqy8_TwPkbHJSsZH1Xz-MygUbnurqpZZwvU", null)]
    [TestFixture("Mw2JX3NUZqy8_TwPkbHJSsZH1Xz-MygUbnurqpZZwvU", "application/pdf")]
    public class DocumentDownloadTests : DocumentTestBase<DocumentDownload>
    {
        private readonly string _documentId;
        private readonly string _requestedContentType;
        private CompaniesHouseClientResponse<DocumentDownload> _result;

        public DocumentDownloadTests(string documentId, string requestedContentType)
        {
            _documentId = documentId;
            requestedContentType = _requestedContentType;
        }

        [SetUp]
        protected override async Task When() => await DownloadingDocument();

        private async Task DownloadingDocument() => _result = await Client.DownloadDocumentAsync(_documentId, contentType: _requestedContentType);

        [Test]
        public async Task ThenDocumentContentIsNotEmpty()
        {
            using var memoryStream = new MemoryStream();
            await _result.Data.Content.CopyToAsync(memoryStream);

            Assert.AreEqual(_result.Data.ContentLength, memoryStream.Length);
            Assert.That(_result.Data.ContentType, Is.Not.Null.Or.Not.Empty);
        }
    }

    [TestFixture]
    public class DocumentDownloadTestsInvalid : DocumentTestBase<DocumentDownload>
    {
        private const string DocumentId = "000000000000000000000000000000";
        private CompaniesHouseClientResponse<DocumentDownload> _result;

        [SetUp]
        protected override async Task When() => await DownloadingDocument();

        private async Task DownloadingDocument() => _result = await Client.DownloadDocumentAsync(DocumentId);

        [Test]
        public void ThenDocumentDataIsNull() => Assert.Null(_result.Data);
    }

    [TestFixture]
    public class DocumentDownloadTestsInvalidContentType : DocumentTestBase<DocumentDownload>
    {
        private const string _documentId = "";
        private const string _requestedContentType = "";
        
        [SetUp]
        protected override async Task When()
        {

        }

        [Test]
        public void ThenThrows406Error()
        {
            Assert.ThrowsAsync<System.Exception>(async () => await Client.DownloadDocumentAsync(_documentId, contentType: _requestedContentType));
        }
    }
}
