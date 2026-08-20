namespace B2B.AccountInformation.Core.Interfaces;

public interface IRequestContextAccessor
{
    string? AccessToken { get; }
}