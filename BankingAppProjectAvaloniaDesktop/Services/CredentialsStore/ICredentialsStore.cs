using System.Threading.Tasks;

namespace BankingAppProjectAvaloniaDesktop.Interfaces;

public interface ICredentialsStore
{
    Task StoreAsync(string token);
    Task<string?> GetAsync();
    Task DeleteAsync();
}
