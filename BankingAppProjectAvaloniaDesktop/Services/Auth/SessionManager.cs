using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Interfaces;
using BankingAppProjectAvaloniaDesktop.Models;

namespace BankingAppProjectAvaloniaDesktop.Services.Auth;

public class SessionManager : ISessionManager
{
    private static string? _accessToken;
    private static DateTime? _expiration;
    private static string? _refreshToken;
    private readonly HttpClient _httpClient;
    private readonly ICredentialsStore _credentialsStore;

    public SessionManager(HttpClient httpClient, ICredentialsStore credentialsStore)
    {
        _httpClient = httpClient;
        _credentialsStore = credentialsStore;
    }
    public static AuthModel GetAuthModel()
    {
        var authModel = new AuthModel();
        authModel.AccessToken = _accessToken;
        authModel.Expiration = _expiration;
        authModel.RefreshToken = _refreshToken;
        return authModel;
    }
    public async Task<SimpleDialogModel<string>> GetAccessTokenAsync()
    {
        try
        {
            if (_expiration >= DateTime.UtcNow || _accessToken is not null)
                return new SimpleDialogModel<string>{ StatusMessage = "Success", Data = _accessToken };

            if (_refreshToken is null)
                return new SimpleDialogModel<string>{ Data = null };

            var response = await _httpClient.GetFromJsonAsync<ApiResponseModel<AuthModel>>("accounts/renew_token");
            SimpleDialogModel<AuthModel> simpleRes = ApiResponse.Simplify(response);
            if (simpleRes.Data is not null)
            {
                _accessToken = simpleRes.Data.AccessToken;
                _expiration = simpleRes.Data.Expiration;

                string refreshToken = simpleRes.Data.RefreshToken!;
                _refreshToken = refreshToken;
                await _credentialsStore.StoreAsync(refreshToken);
                return new SimpleDialogModel<string>{StatusMessage = "Success", Data = _accessToken};
            }
            await _credentialsStore.DeleteAsync();
            return new SimpleDialogModel<string>{ Data = null };
        }
        catch (Exception ex)
        {
            //await _credentialsStore.DeleteAsync();
            return new SimpleDialogModel<string> { Title = "Exception", StatusMessage = "Error", Message = ex.Message };
        }
    }
    public string? GetRefreshToken()
    {
        return _refreshToken;
    }
    public async Task InitializeSession()
    {
        _refreshToken = await _credentialsStore.GetAsync() ?? null;
    }
    public void Session()
    {
        Console.WriteLine($"AccessToken: {_accessToken}");
        Console.WriteLine($"Expiration: {_expiration}");
        Console.WriteLine($"RefreshToken: {_refreshToken}");
    }
    public async Task<SimpleDialogModel<object>> TerminateSession()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponseModel<object>>("accounts/logout");
            await _credentialsStore.DeleteAsync();
            _accessToken = null;
            _expiration = null;
            _refreshToken = null;
            return ApiResponse.Simplify(response);
        }
        catch (Exception ex)
        {
            _accessToken = null;
            _expiration = null;
            _refreshToken = null;
            await _credentialsStore.DeleteAsync();
            return new SimpleDialogModel<object> { Title = "Exception", StatusMessage = "Error", Message = ex.Message };
        }
    }
    public void SetSession(AuthModel authModel)
    {
        _accessToken = authModel.AccessToken;
        _expiration = authModel.Expiration;
        var token = authModel.RefreshToken;
        _refreshToken = token;
        _credentialsStore.StoreAsync(token!);
    }
}