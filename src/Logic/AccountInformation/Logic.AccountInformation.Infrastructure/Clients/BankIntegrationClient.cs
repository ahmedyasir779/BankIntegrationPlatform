using System.Net.Http.Json;
using Logic.AccountInformation.Core.Interfaces;
// using Logic.AccountInformation.Shared.Requests;
// using Logic.AccountInformation.Shared.Responses;
using Logic.AccountInformation.Infrastructure.Common;
using Logic.AccountInformation.Shared.Contracts;

namespace Logic.AccountInformation.Infrastructure.Clients;

public class BankIntegrationClient : IBankIntegrationClient
{
    private readonly HttpClient _httpClient;
    private readonly IRequestContextAccessor _requestContext;

    public BankIntegrationClient(
    HttpClient httpClient,
    IRequestContextAccessor requestContext)
    {
        _httpClient = httpClient;
        _requestContext = requestContext;
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(
        GetBalanceRequest request)
    {
        _httpClient.DefaultRequestHeaders.Remove("Authorization");

        if (!string.IsNullOrWhiteSpace(_requestContext.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Add(
                "Authorization",
                _requestContext.AccessToken);
        }

        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/balance",
            request);

        response.EnsureSuccessStatusCode();

        var apiResponse =
            await response.Content.ReadFromJsonAsync<ApiResponse<GetBalanceResponse>>();

        return apiResponse!.Data!;
    }
}