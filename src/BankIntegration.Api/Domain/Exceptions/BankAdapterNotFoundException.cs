public class BankAdapterNotFoundException : Exception
{
    public BankAdapterNotFoundException(string bankCode)
        : base($"No adapter registered for bank '{bankCode}'.")
    {
    }

    
}