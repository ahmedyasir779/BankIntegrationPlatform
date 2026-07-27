using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;
using BankIntegrationPlatform.Infrastructure.External.Adapters;
using BankIntegrationPlatform.Infrastructure.External.AdapterRegistry;
using BankIntegrationPlatform.Common;

namespace BankIntegrationPlatform.Application.Services;

public class BankService : IBankService
{
    private readonly AdapterRegistry _adapterRegistry;
    private readonly ILogger<BankService> _logger;

    private readonly IRequestContextAccessor _requestContext;


    public BankService(
        AdapterRegistry adapterRegistry,
        ILogger<BankService> logger,
        IRequestContextAccessor requestContext)
    {
        _adapterRegistry = adapterRegistry;
        _logger = logger;
        _requestContext = requestContext;
    }

    public async Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        var context = _requestContext.Context;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation(
            "Processing balance request. CorrelationId: {CorrelationId}, MessageId: {MessageId}, Bank: {BankCode}, Account: {AccountNumber}",
            context.CorrelationId,
            context.MessageId,
            request.BankCode,
            request.AccountNumber);


        var adapter = _adapterRegistry.GetAdapter(request.BankCode);

        _logger.LogInformation(
            "Adapter selected. CorrelationId: {CorrelationId}, Adapter: {Adapter}",
            context.CorrelationId,
            adapter.GetType().Name);

        var response = await adapter.GetBalanceAsync(request);

        stopwatch.Stop();

        _logger.LogInformation(
            "Balance request completed. CorrelationId: {CorrelationId}, MessageId: {MessageId}, Bank: {BankCode}, Account: {AccountNumber}, Balance: {Balance}, Duration: {Duration} ms",
            context.CorrelationId,
            context.MessageId,
            request.BankCode,
            response.AccountNumber,
            response.Balance,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}