namespace Logic.AccountInformation.Core.Interfaces;

public interface IRequestContextAccessor
{
    string? AccessToken { get; }
}