using Microsoft.AspNetCore.Mvc;
using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;

namespace BankIntegrationPlatform.Controllers;

[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
{
    private readonly IBankService _bankService;
    
    public BalanceController(IBankService bankService)
    {
        _bankService = bankService;
    } 

    [HttpPost]
    public ActionResult<BalanceResponse> GetBalance(BalanceRequest request)
    {
        BalanceResponse response = _bankService.GetBalance(request);

        return Ok(response);
    }
}