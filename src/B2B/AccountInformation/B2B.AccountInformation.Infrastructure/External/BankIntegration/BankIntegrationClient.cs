using System.Net.Http.Json;
using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Shared.Contracts;

namespace B2B.AccountInformation.Infrastructure.External.BankIntegration;

public class BankIntegrationClient : IBankIntegrationClient
{
    private readonly HttpClient _httpClient;

    public BankIntegrationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(
        GetBalanceRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/balance",
            request);

        response.EnsureSuccessStatusCode();

        var apiResponse =
            await response.Content.ReadFromJsonAsync<ApiResponse<GetBalanceResponse>>();

        return apiResponse!.Data!;
    }
}