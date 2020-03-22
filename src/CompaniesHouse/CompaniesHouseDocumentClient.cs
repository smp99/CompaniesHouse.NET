using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CompaniesHouse.Response.Document;
using CompaniesHouse.UriBuilders;

namespace CompaniesHouse
{
    public class CompaniesHouseDocumentClient : ICompaniesHouseDocumentClient
    {
        private readonly HttpClient _httpClient;
        private readonly IDocumentUriBuilder _documentUriBuilder;

        public CompaniesHouseDocumentClient(HttpClient httpClient, IDocumentUriBuilder documentUriBuilder)
        {
            _httpClient = httpClient;
            _documentUriBuilder = documentUriBuilder;
        }

        public async Task<CompaniesHouseClientResponse<DocumentDownload>> DownloadDocumentAsync(string documentId, CancellationToken cancellationToken = default, string contentType = default)
        {
            var requestUri = _documentUriBuilder.WithContent().Build(documentId);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            if (!string.IsNullOrEmpty(contentType))
            {
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            }

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.NotFound)
                response.EnsureSuccessStatusCode();

            var data = response.IsSuccessStatusCode
                ? new DocumentDownload
                {
                    Content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false),
                    ContentLength = response.Content.Headers.ContentLength,
                    ContentType = response.Content.Headers.ContentType.MediaType
                }
                : null;

            return new CompaniesHouseClientResponse<DocumentDownload>(data);
        }
    }
}