using Microsoft.AspNetCore.Mvc;
using BankMockServer.Api.Models;

namespace BankMockServer.Api.Controllers;

[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
{
    private readonly ILogger<BalanceController> _logger;

    public BalanceController(ILogger<BalanceController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<BalanceResponse>> GetBalance(
        BalanceRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received balance request for {AccountNumber}",
            request.AccountNumber);

        // Simulate bank processing time
        await Task.Delay(1000, cancellationToken);

        var response = new BalanceResponse
        {
            AccountNumber = request.AccountNumber,
            Balance = 74412.75m,
            Currency = "SAR"
        };

        return Ok(response);
    }
}