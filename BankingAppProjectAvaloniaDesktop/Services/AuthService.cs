using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Services.Auth;

namespace BankingAppProjectAvaloniaDesktop.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ISessionManager _sessionManager;

    public AuthService(HttpClient httpClient, ISessionManager sessionManager)
    {
        _httpClient = httpClient;
        _sessionManager = sessionManager;
    }
    public async Task<SimpleDialogModel<AuthModel>> LoginAsync(string? email, string? password)
    {
        try
        {
            var loginData = new
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("accounts/login", loginData);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseModel<AuthModel>>();
            var res = ApiResponse.Simplify(content);
            if (res.StatusMessage == "Success" && res.Data is not null)
            {
                _sessionManager.SetSession(res.Data);
                return new SimpleDialogModel<AuthModel> { StatusMessage = res.StatusMessage };
            }
            return new SimpleDialogModel<AuthModel> { Status = res.Status, StatusMessage = res.StatusMessage, Message = res.Message };
        }
        catch (Exception ex)
        {
            return new SimpleDialogModel<AuthModel> { Title = "Exception", StatusMessage = "Error", Message = ex.Message };
        }
    }

    public async Task<SimpleDialogModel> CreateAccountAsync(string? name, string? email, string? password)
    {
        try
        {
            var AccountData = new
            {
                Email = email,
                Password = password,
                Name = name
            };
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5223/api/accounts/sign_up", AccountData);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseModel>();
            return ApiResponse.Simplify(content);
        }
        catch (Exception ex)
        {
            return new SimpleDialogModel { Title = "Exception", StatusMessage = "Error", Message = ex.Message };
        }
    }

    public async Task<SimpleDialogModel<string>> ResumeSession()
    {
        try
        {
            await _sessionManager.InitializeSession();
            SimpleDialogModel<string> res = await _sessionManager.GetAccessTokenAsync();
            return res;
        }catch (Exception ex){
            return new SimpleDialogModel<string> { Title = "Exception", StatusMessage = "Error", Message = ex.Message, Data = "" };
        }
    }
}
