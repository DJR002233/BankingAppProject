using Microsoft.AspNetCore.Mvc;
using BankingAppProjectWebAPI.Requests;
using BankingAppProjectWebAPI.Services.DependencyInjection;
using AutoMapper;
using BankingAppProjectWebAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BankingAppProjectWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly IMapper _mapper;
    public AccountsController(AccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        Response<object>? res = await _accountService.LoginAccountAsync(request);
        if (!String.IsNullOrWhiteSpace(res?.Data?.ToString()))
            return Ok(res);

        return Unauthorized(res);
    }

    [HttpPost("sign_up")]
    public async Task<IActionResult> AddAccount([FromBody] CreateAccountRequest request)
    {
        var account = _mapper.Map<AccountInformation>(request);
        account.BankAccount = new BankAccountInformation { Name = request.Name };

        Response<object> res = await _accountService.CreateAccountAsync(account);
        if (res.StatusMessage == "Success")
        {
            return Ok(res);
        }
        else if (res.StatusMessage == "Failed") return Conflict(res);

        return Unauthorized(res);
    }

    [HttpGet("renew_token")]
    public async Task<IActionResult> RenewToken([FromHeader(Name = "X-Refresh-Token")] string refreshToken)
    {
        //return Ok(new { StatusMessage = "Error", Message = refreshToken });
        //int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Response<object> res = await _accountService.GetAccessTokenAsync(refreshToken);
        //Console.WriteLine(res.Message);
        //return Ok(res);
        //Thread.Sleep(10000);
        if (res.StatusMessage == "Success")
            return Ok(res);
        if (res.StatusMessage == "Failed")
            return Ok(res);

        return Unauthorized(res);
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout([FromHeader(Name = "X-Refresh-Token")] string refreshToken)
    {
        Response<object>? res = await _accountService.ClearTokenAsync(refreshToken);
        if (res.StatusMessage == "Success" || res.StatusMessage == "Failed")
            return Ok(res);

        return Unauthorized(res);/**/
    }

}
