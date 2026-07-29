namespace BankIntegration.Api.Common;

public interface IRequestContextAccessor
{
    RequestContext Context { get; set; }
}