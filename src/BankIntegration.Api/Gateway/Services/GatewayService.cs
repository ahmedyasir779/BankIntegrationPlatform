using BankIntegration.Api.Domain.Models;
using BankIntegration.Api.Gateway.Contracts;
using BankIntegration.Api.INT.Routing;

namespace BankIntegration.Api.Gateway.Services;

public class GatewayService : IGatewayService
{
    private readonly AdapterRegistry _adapterRegistry;

    public GatewayService(AdapterRegistry adapterRegistry)
    {
        _adapterRegistry = adapterRegistry;
    }

    public async Task<GatewayBalanceResponse> GetBalanceAsync(GatewayBalanceRequest request)
    {
        var adapter = _adapterRegistry.GetAdapter(request.BankCode);

        var adapterRequest = new BalanceRequest
        {
            BankCode = request.BankCode,
            AccountNumber = request.AccountNumber
        };

        var response = await adapter.GetBalanceAsync(adapterRequest);

        return new GatewayBalanceResponse
        {
            AccountNumber = response.AccountNumber,
            Balance = response.Balance,
            Currency = response.Currency
        };
    }
}