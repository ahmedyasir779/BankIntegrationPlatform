using Microsoft.AspNetCore.Mvc;
using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;
using Microsoft.Extensions.Options;
using BankIntegrationPlatform.Infrastructure.Configurations;

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

        return Ok(response);
    }

    [HttpGet("config")]
    public IActionResult GetConfiguration()
    {
        return Ok(_bankOptions);
    }
}