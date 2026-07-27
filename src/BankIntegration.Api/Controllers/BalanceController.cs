using Microsoft.AspNetCore.Mvc;
using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;
using Microsoft.Extensions.Options;
using BankIntegrationPlatform.Infrastructure.Configurations;
using BankIntegrationPlatform.Domain.Messages;
using BankIntegrationPlatform.Common;

namespace BankIntegrationPlatform.Controllers;

[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
{
    private readonly IBankService _bankService;
    private readonly BankOptions _bankOptions;

    private readonly IRequestContextAccessor _requestContext;

    public BalanceController(
        IBankService bankService,
        IOptions<BankOptions> option,
        IRequestContextAccessor requestContext)
    {
        _bankService = bankService;
        _bankOptions = option.Value;
        _requestContext = requestContext;
    }

    [HttpPost]
    public async Task<ActionResult<BalanceResponse>> GetBalance(BalanceRequest request)
    {
        BalanceResponse response = await _bankService.GetBalanceAsync(request);

        // Guid correlationId = Guid.Empty;

        // if (HttpContext.Items.TryGetValue(HttpContextKeys.CorrelationId, out var value))
        // {
        //     Guid.TryParse(value?.ToString(), out correlationId);
        // }

        var apiResponse = new ApiResponse<BalanceResponse>
        {
            Header = new ResponseHeader
            {
                CorrelationId = _requestContext.Context.CorrelationId,
                MessageId = _requestContext.Context.MessageId,
                TimestampUtc = _requestContext.Context.RequestTimeUtc,

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