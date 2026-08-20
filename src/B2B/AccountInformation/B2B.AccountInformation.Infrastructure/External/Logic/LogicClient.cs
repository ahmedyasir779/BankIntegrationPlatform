using System.Net.Http.Json;
using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Shared.Contracts;
using B2B.AccountInformation.Infrastructure.Common;

namespace B2B.AccountInformation.Infrastructure.External.Logic;

public class LogicClient : ILogicClient
{
    private readonly HttpClient _httpClient;
    private readonly IRequestContextAccessor _requestContext;

    public LogicClient(
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