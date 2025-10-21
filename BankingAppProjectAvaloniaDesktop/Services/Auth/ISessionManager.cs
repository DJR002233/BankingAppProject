using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Models;

namespace BankingAppProjectAvaloniaDesktop.Services.Auth;

public interface ISessionManager
{
    Task<SimpleDialogModel<string>> GetAccessTokenAsync();
    string? GetRefreshToken();
    Task<SimpleDialogModel<object>> TerminateSession();
    void SetSession(AuthModel authModel);
    Task InitializeSession();
}
