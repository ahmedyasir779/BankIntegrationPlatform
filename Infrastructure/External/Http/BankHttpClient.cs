using System.Text.Json;

namespace BankIntegrationPlatform.Infrastructure.External.Http;

public class BankHttpClient : IBankHttpClient
{
    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        // Simulate network latency
        await Task.Delay(300, cancellationToken);

        object response = new
        {
            AccountNumber = "123456789",
            Balance = 9999.99m,
            Currency = "SAR"
        };

        string json = JsonSerializer.Serialize(response);

        return JsonSerializer.Deserialize<TResponse>(json)!;
    }
}