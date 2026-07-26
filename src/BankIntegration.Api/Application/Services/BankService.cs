using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;
using BankIntegrationPlatform.Infrastructure.External.Adapters;
using BankIntegrationPlatform.Infrastructure.External.AdapterRegistry;

namespace BankIntegrationPlatform.Application.Services;

public class BankService : IBankService
{
    private readonly AdapterRegistry _adapterRegistry;
    private readonly ILogger<BankService> _logger;

    public BankService(AdapterRegistry adapterRegistry, ILogger<BankService> logger)
    {
        _adapterRegistry = adapterRegistry;
        _logger = logger;
    }

    public async Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        _logger.LogInformation(
        "Processing balance request for Bank: {BankCode}, Account: {AccountNumber}",
        request.BankCode,
        request.AccountNumber);

        var adapter = _adapterRegistry.GetAdapter(request.BankCode);

        _logger.LogInformation(
        "Selected adapter: {Adapter}",
        adapter.GetType().Name);

        var response = await adapter.GetBalanceAsync(request);

        _logger.LogInformation(
            "Balance request completed successfully for Bank: {BankCode}",
            request.BankCode);

        return response;
    }
}