using BankIntegration.Api.Application.Interfaces;
using BankIntegration.Api.Common;
using BankIntegration.Api.Domain.Models;
using BankIntegration.Api.Gateway.Contracts;
using BankIntegration.Api.Gateway.Services;

namespace BankIntegration.Api.Logic.Services;

public class BankService : IBankService
{
    //private readonly AdapterRegistry _adapterRegistry;
    private readonly IGatewayService _gatewayService;
    private readonly ILogger<BankService> _logger;

    private readonly IRequestContextAccessor _requestContext;


    public BankService(
    IGatewayService gatewayService,
    ILogger<BankService> logger,
    IRequestContextAccessor requestContext)
    {
        _gatewayService = gatewayService;
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


        //var adapter = _adapterRegistry.GetAdapter(request.BankCode);

        //_logger.LogInformation(
        //    "Adapter selected. CorrelationId: {CorrelationId}, Adapter: {Adapter}",
        //    context.CorrelationId,
        //    adapter.GetType().Name);

        //var response = await adapter.GetBalanceAsync(request);
        //var response = await _gatewayService.GetBalanceAsync(request);


        var gatewayRequest = new GatewayBalanceRequest
        {
            BankCode = request.BankCode,
            AccountNumber = request.AccountNumber
        };

        var gatewayResponse = await _gatewayService.GetBalanceAsync(gatewayRequest);

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

        return new BalanceResponse
        {
            AccountNumber = gatewayResponse.AccountNumber,
            Balance = gatewayResponse.Balance,
            Currency = gatewayResponse.Currency
        };

    }
}