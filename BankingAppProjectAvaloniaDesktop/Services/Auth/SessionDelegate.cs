using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Servicesl;

namespace BankingAppProjectAvaloniaDesktop.Services.Auth;

public class SessionHeaderHandler : DelegatingHandler
{
    private readonly ISessionManager _sessionManager;
    private readonly IAuthEventService _authEvent;
    public SessionHeaderHandler(ISessionManager sessionManager, IAuthEventService authEvent)
    {
        _sessionManager = sessionManager;
        _authEvent = authEvent;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _sessionManager.GetAccessTokenAsync();
        var refreshToken = _sessionManager.GetRefreshToken();

        if (!string.IsNullOrEmpty(accessToken.Data))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Data);
        }
        if (!string.IsNullOrEmpty(refreshToken))
        {
            request.Headers.Add("X-Refresh-Token", refreshToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _sessionManager.TerminateSession();
            _authEvent.TriggerUnauthorized();
            await DialogBox.Show("Error 401", "Session Expired!\nPlease login again...");
        }

        return response;
    }
}
