using BankIntegration.Api.Application.Interfaces;
using BankIntegration.Api.Domain.Models;
using BankIntegration.Api.Infrastructure.External.Adapters;
using BankIntegration.Api.Infrastructure.External.AdapterRegistry;
using BankIntegration.Api.Common;

namespace BankIntegration.Api.Application.Services;

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
            """
            Request started.
            CorrelationId: {CorrelationId}
            MessageId: {MessageId}
            Service: {Service}
            Method: {Method}
            Path: {Path}
            Bank: {Bank}
            Account: {Account}
            """,
            _requestContext.Context.CorrelationId,
            _requestContext.Context.MessageId,
            _requestContext.Context.ServiceName,
            _requestContext.Context.HttpMethod,
            _requestContext.Context.RequestPath,
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
            """
            Request completed.
            CorrelationId: {CorrelationId}
            MessageId: {MessageId}
            Duration: {Duration} ms
            """,
            _requestContext.Context.CorrelationId,
            _requestContext.Context.MessageId,
            stopwatch.ElapsedMilliseconds);


        return response;
    }
}