using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace B2B.AccountInformation.Api.Controllers;

[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
{
    private readonly IGetBalanceService _service;

    public BalanceController(IGetBalanceService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<GetBalanceResponse>> GetBalance(
        [FromBody] GetBalanceRequest request)
    {
        var result = await _service.GetBalanceAsync(request);

        return Ok(result);
    }
}