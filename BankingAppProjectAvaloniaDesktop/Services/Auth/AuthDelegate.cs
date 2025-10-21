using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BankingAppProjectAvaloniaDesktop.Services.Auth;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ISessionManager _sessionManager;

    public AuthHeaderHandler(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _sessionManager.GetAccessTokenAsync();

        if (!string.IsNullOrEmpty(accessToken.Data))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Data);
        }

        var res = await base.SendAsync(request, cancellationToken);

        return res;
    }
}
