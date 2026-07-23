using Microsoft.AspNetCore.Mvc;
using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;
using Microsoft.Extensions.Options;
using BankIntegrationPlatform.Infrastructure.Configurations;
using BankIntegrationPlatform.Domain.Messages;

namespace BankIntegrationPlatform.Controllers;

[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
{
    private readonly IBankService _bankService;
    private readonly BankOptions _bankOptions;

    public BalanceController(
        IBankService bankService,
        IOptions<BankOptions> option)
    {
        _bankService = bankService;
        _bankOptions = option.Value;
    } 

    [HttpPost]
    public async Task<ActionResult<BalanceResponse>> GetBalance(BalanceRequest request)
    {
        BalanceResponse response = await _bankService.GetBalanceAsync(request);
        
        var apiResponse = new ApiResponse<BalanceResponse>
        {
            Header = new ResponseHeader
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid(),
                TimestampUtc = DateTime.UtcNow,

                Status = new ResponseStatus
                {
                    StatusType = "Success",
                    StatusCode = "000",
                    StatusDescription = "Request completed successfully."
                }
            },

            Data = response
        };

        return Ok(apiResponse);
    }

    [HttpGet("config")]
    public IActionResult GetConfiguration()
    {
        return Ok(_bankOptions);
    }
}