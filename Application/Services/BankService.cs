using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;
using BankIntegrationPlatform.Infrastructure.External.Adapters;
using BankIntegrationPlatform.Infrastructure.External.AdapterRegistry;

namespace BankIntegrationPlatform.Application.Services;

public class BankService : IBankService
{
    private readonly AdapterRegistry _adapterRegistry;

    public BankService(AdapterRegistry adapterRegistry)
    {
        _adapterRegistry = adapterRegistry;
    }

    public async Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        IBankAdapter adapter =
            _adapterRegistry.GetAdapter(request.BankCode);

        return await adapter.GetBalanceAsync(request);
    }
}