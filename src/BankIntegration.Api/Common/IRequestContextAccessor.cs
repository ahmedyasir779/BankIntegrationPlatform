namespace BankIntegrationPlatform.Common;

public interface IRequestContextAccessor
{
    RequestContext Context { get; }
}