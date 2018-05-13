﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CompaniesHouse.Request;
using CompaniesHouse.Response.CompanyFiling;
using CompaniesHouse.Response.CompanyProfile;
using CompaniesHouse.Response.DocumentMetadata;
using CompaniesHouse.Response.Insolvency;
using CompaniesHouse.Response.Officers;
using CompaniesHouse.Response.Search.AllSearch;
using CompaniesHouse.Response.Search.CompanySearch;
using CompaniesHouse.Response.Search.DisqualifiedOfficersSearch;
using CompaniesHouse.Response.Search.OfficerSearch;
using CompaniesHouse.UriBuilders;

namespace CompaniesHouse
{
    public class CompaniesHouseClient : ICompaniesHouseClient, IDisposable
    {
        private readonly ICompaniesHouseSearchClient _companiesHouseSearchClient;
        private readonly ICompaniesHouseCompanyProfileClient _companiesHouseCompanyProfileClient;
        private readonly ICompaniesHouseCompanyFilingHistoryClient _companiesHouseCompanyFilingHistoryClient;
        private readonly ICompaniesHouseOfficersClient _companiesHouseOfficersClient;
        private readonly ICompaniesHouseCompanyInsolvencyInformationClient _companiesHouseCompanyInsolvencyInformationClient;
        private readonly ICompaniesHouseDocumentMetadataClient _companiesHouseDocumentMetadataClient;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _documentHttpClient;

        public CompaniesHouseClient(ICompaniesHouseSettings settings)
        {
            var companiesHouseHttpClientFactory = new CompaniesHouseHttpClientFactory(settings);
            _httpClient = companiesHouseHttpClientFactory.CreateHttpClient();
            var companiesHouseDocumentHttpClientFactory = new CompaniesHouseDocumentHttpClientFactory(settings);
            _documentHttpClient = companiesHouseDocumentHttpClientFactory.CreateHttpClient();

            _companiesHouseSearchClient = new CompaniesHouseSearchClient(_httpClient, new SearchUriBuilderFactory());
            _companiesHouseCompanyProfileClient = new CompaniesHouseCompanyProfileClient(_httpClient, new CompanyProfileUriBuilder());
            _companiesHouseCompanyFilingHistoryClient = new CompaniesHouseCompanyFilingHistoryClient(_httpClient, new CompanyFilingHistoryUriBuilder());
            _companiesHouseOfficersClient = new CompaniesHouseOfficersClient(_httpClient, new OfficersUriBuilder());
            _companiesHouseCompanyInsolvencyInformationClient = new CompaniesHouseCompanyInsolvencyInformationClient(_httpClient);
            _companiesHouseDocumentMetadataClient = new CompaniesHouseDocumentMetadataClient(_documentHttpClient, new DocumentMetadataUriBuilder());
        }

        public Task<CompaniesHouseClientResponse<CompanySearch>> SearchCompanyAsync(SearchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseSearchClient.SearchAsync<CompanySearch>(request, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<OfficerSearch>> SearchOfficerAsync(SearchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseSearchClient.SearchAsync<OfficerSearch>(request, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<DisqualifiedOfficerSearch>> SearchDisqualifiedOfficerAsync(SearchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseSearchClient.SearchAsync<DisqualifiedOfficerSearch>(request, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<AllSearch>> SearchAllAsync(SearchRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseSearchClient.SearchAsync<AllSearch>(request, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<CompanyProfile>> GetCompanyProfileAsync(string companyNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseCompanyProfileClient.GetCompanyProfileAsync(companyNumber, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<CompanyFilingHistory>> GetCompanyFilingHistoryAsync(string companyNumber, int startIndex = 0, int pageSize = 25, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseCompanyFilingHistoryClient.GetCompanyFilingHistoryAsync(companyNumber, startIndex, pageSize, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<Officers>> GetOfficersAsync(string companyNumber, int startIndex = 0, int pageSize = 25, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseOfficersClient.GetOfficersAsync(companyNumber, startIndex, pageSize, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<CompanyInsolvencyInformation>> GetCompanyInsolvencyInformationAsync(string companyNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _companiesHouseCompanyInsolvencyInformationClient.GetCompanyInsolvencyInformationAsync(companyNumber, cancellationToken);
        }

        public Task<CompaniesHouseClientResponse<DocumentMetadata>> GetDocumentMetadataAsync(string documentId, CancellationToken cancellationToken)
        {
            return _companiesHouseDocumentMetadataClient.GetDocumentMetadataAsync(documentId, cancellationToken);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _documentHttpClient.Dispose();
        }

    }
}
