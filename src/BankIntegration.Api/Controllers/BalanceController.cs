using Microsoft.AspNetCore.Mvc;
using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;
using Microsoft.Extensions.Options;
using BankIntegrationPlatform.Infrastructure.Configurations;
using BankIntegrationPlatform.Domain.Messages;
using BankIntegrationPlatform.Common;
using BankIntegrationPlatform.Application.Common;

namespace BankIntegrationPlatform.Controllers;

[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
{
    private readonly IBankService _bankService;
    private readonly BankOptions _bankOptions;

    // private readonly IRequestContextAccessor _requestContext;

    private readonly IApiResponseFactory _responseFactory;

    public BalanceController(
       IBankService bankService,
       IOptions<BankOptions> option,
       IApiResponseFactory responseFactory)
    {
        _bankService = bankService;
        _bankOptions = option.Value;
        _responseFactory = responseFactory;
    }

    [HttpPost]
    public async Task<ActionResult<BalanceResponse>> GetBalance(BalanceRequest request)
    {
        BalanceResponse response = await _bankService.GetBalanceAsync(request);

        return Ok(_responseFactory.Success(response));
    }

    [HttpGet("config")]
    public IActionResult GetConfiguration()
    {
        return Ok(_bankOptions);
    }
}