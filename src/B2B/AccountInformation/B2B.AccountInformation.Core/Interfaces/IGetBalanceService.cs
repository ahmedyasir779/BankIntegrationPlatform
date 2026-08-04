using B2B.AccountInformation.Shared.Contracts;

namespace B2B.AccountInformation.Core.Interfaces;

public interface IGetBalanceService
{
    Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);
}