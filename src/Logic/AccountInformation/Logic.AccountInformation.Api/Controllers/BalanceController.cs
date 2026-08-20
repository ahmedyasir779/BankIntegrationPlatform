using Logic.AccountInformation.Core.Interfaces;
using Logic.AccountInformation.Shared.Requests;
using Logic.AccountInformation.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Logic.AccountInformation.Shared.Contracts;

namespace Logic.AccountInformation.Api.Controllers;

[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
{
    private readonly ILogicService _logicService;

    public BalanceController(ILogicService logicService)
    {
        _logicService = logicService;
    }

    [HttpPost]
    public async Task<ActionResult<GetBalanceResponse>> GetBalance(
        GetBalanceRequest request)
    {
        var response = await _logicService.GetBalanceAsync(request);

        return Ok(new ApiResponse<GetBalanceResponse>
        {
            Data = response
        });
    }
}