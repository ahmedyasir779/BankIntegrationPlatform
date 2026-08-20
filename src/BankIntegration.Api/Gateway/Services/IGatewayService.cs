using BankIntegration.Api.Gateway.Contracts;

namespace BankIntegration.Api.Gateway.Services;

public interface IGatewayService
{
    Task<GatewayBalanceResponse> GetBalanceAsync(
        GatewayBalanceRequest request);
}