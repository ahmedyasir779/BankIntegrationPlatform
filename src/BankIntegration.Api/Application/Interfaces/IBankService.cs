using BankIntegration.Api.Domain.Models;

namespace BankIntegration.Api.Application.Interfaces;

public interface IBankService
{
    Task<BalanceResponse> GetBalanceAsync(BalanceRequest request);
}