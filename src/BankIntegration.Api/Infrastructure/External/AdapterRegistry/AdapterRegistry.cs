using BankIntegration.Api.Infrastructure.External.Adapters;

namespace BankIntegration.Api.Infrastructure.External.AdapterRegistry;

public class AdapterRegistry
{
    private readonly Dictionary<string, IBankAdapter> _adapters;

    public AdapterRegistry(IEnumerable<IBankAdapter> adapters)
    {
        _adapters = adapters.ToDictionary(
            adapter => adapter.BankCode,
            adapter => adapter,
            StringComparer.OrdinalIgnoreCase);
    }

    public IBankAdapter GetAdapter(string bankCode)
    {
        if (_adapters.TryGetValue(bankCode, out var adapter))
        {
            return adapter;
        }

        throw new BankAdapterNotFoundException(bankCode);
    }
}