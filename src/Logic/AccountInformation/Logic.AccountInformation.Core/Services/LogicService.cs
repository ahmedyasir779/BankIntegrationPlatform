using Logic.AccountInformation.Core.Interfaces;
// using Logic.AccountInformation.Shared.Requests;
// using Logic.AccountInformation.Shared.Responses;
using Logic.AccountInformation.Shared.Contracts;

namespace Logic.AccountInformation.Core.Services;

public class LogicService : ILogicService
{
    private readonly IBankIntegrationClient _bankIntegrationClient;

    private readonly ILogger<LogicService> _logger;

    public LogicService(
        IBankIntegrationClient bankIntegrationClient,
        ILogger<ClientService> logger)
    {
        _bankIntegrationClient = bankIntegrationClient;
        _logger = logger;
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(
        GetBalanceRequest request)
    {
        _logger.LogInformation(
            """
            Request started.
            """);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var response = await _bankIntegrationClient.GetBalanceAsync(request);

        stopwatch.Stop();

         _logger.LogInformation(
            """
            Request completed.
            Duration: {Duration} ms
            """,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}