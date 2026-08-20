using System.Net.Http.Json;

namespace BankIntegration.Api.Gateway.Http;

public class BankHttpClient : IBankHttpClient
{
    private readonly ILogger<BankHttpClient> _logger;
    private readonly HttpClient _httpClient;

    public BankHttpClient(
        HttpClient httpClient,
        ILogger<BankHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("POST {Url}", url);

        var response = await _httpClient.PostAsJsonAsync(
            url,
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken);

        if (result is null)
            throw new Exception("Bank returned an empty response.");

        return result;
    }
}