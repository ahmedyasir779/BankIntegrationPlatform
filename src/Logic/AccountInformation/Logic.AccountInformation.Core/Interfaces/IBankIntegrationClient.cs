// using Logic.AccountInformation.Shared.Requests;
// using Logic.AccountInformation.Shared.Responses;
using Logic.AccountInformation.Shared.Contracts;
namespace Logic.AccountInformation.Core.Interfaces;

public interface IBankIntegrationClient
{
    Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);
}