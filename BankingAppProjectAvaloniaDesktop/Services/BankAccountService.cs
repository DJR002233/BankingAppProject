using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Services.Auth;

namespace BankingAppProjectAvaloniaDesktop.Services;

public class BankAccountService
{
    private readonly HttpClient _httpClient;
    private readonly ISessionManager _sessionManager;

    public BankAccountService(HttpClient httpClient, ISessionManager sessionManager)
    {
        _httpClient = httpClient;
        _sessionManager = sessionManager;
    }
    public async Task<SimpleDialogModel<object>> LogoutAsync()
    {
        var res = await _sessionManager.TerminateSession();
        return res;
    }
    public async Task<SimpleDialogModel<object>> GetBankBalanceAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponseModel<object>>("bankaccounts/balance");
            SimpleDialogModel<object> simpleRes = ApiResponse.Simplify(response);
            return simpleRes;
        }
        catch (Exception ex)
        {
            return new SimpleDialogModel<object> { Title = "Exception", StatusMessage = "Error", Message = ex.Message };
        }
    }

    public async Task<SimpleDialogModel<object>> WithdrawAsync(decimal amount)
    {
        try
        {
            var bodyData = new
            {
                Amount = amount
            };
            var response = await _httpClient.PostAsJsonAsync("bankaccounts/withdraw", bodyData);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseModel<object>>();
            return ApiResponse.Simplify(content);
        }
        catch (Exception ex)
        {
            return new SimpleDialogModel<object> { Title = "Exception", StatusMessage = "Error", Message = ex.Message };
        }
    }

    public async Task<SimpleDialogModel<object>> DepositAsync(decimal amount)
    {
        try
        {
            var bodyData = new
            {
                Amount = amount
            };
            var response = await _httpClient.PostAsJsonAsync("bankaccounts/deposit", bodyData);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseModel<object>>();
            return ApiResponse.Simplify(content);
        }
        catch (Exception ex)
        {
            return new SimpleDialogModel<object> { Title = "Exception", StatusMessage = "Error", Message = ex.Message };
        }
    }

}
