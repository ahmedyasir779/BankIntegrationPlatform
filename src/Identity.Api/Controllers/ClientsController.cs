using Identity.Api.Application.DTOs.Clients;
using Identity.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _clientService.GetAllAsync();

        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _clientService.GetByIdAsync(id);

        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateClientRequest request)
    {
        var client = await _clientService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = client.Id },
            client);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateClientRequest request)
    {
        var client = await _clientService.UpdateAsync(id, request);

        return Ok(client);
    }
}