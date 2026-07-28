using Microsoft.AspNetCore.Mvc;
using BankIntegration.Api.Application.Interfaces;
using BankIntegration.Api.Domain.Models;
using Microsoft.Extensions.Options;
using BankIntegration.Api.Infrastructure.Configurations;
using BankIntegration.Api.Domain.Messages;
using BankIntegration.Api.Common;
using BankIntegration.Api.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace BankIntegration.Api.Controllers;

[ApiController]
[Route("api/v1/balance")]
[Authorize(Policy = "Balance.Read")]
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
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<BalanceResponse>>> GetBalance(
       BalanceRequest request)
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