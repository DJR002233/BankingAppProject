using Microsoft.AspNetCore.Mvc;
using BankingAppProjectWebAPI.Requests;
using BankingAppProjectWebAPI.Services.DependencyInjection;
using AutoMapper;
using BankingAppProjectWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BankingAppProjectWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankAccountsController : ControllerBase
{
    private readonly BankAccountService _bankAccountService;
    private readonly IMapper _mapper;
    public BankAccountsController(BankAccountService bankAccountService, IMapper mapper)
    {
        _bankAccountService = bankAccountService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet("balance")]
    public async Task<IActionResult> AccountBalance()
    {
        /*var headers = Request.Headers
        .Select(h => $"{h.Key}: {h.Value}")
        .ToList();
        var headersText = string.Join(Environment.NewLine, headers);
        return Ok(new { Message = headersText });/**/
        /*if (!Request.Headers.ContainsKey("Authorization"))
            return Ok(new { Message = "No Authorization header found" });

        var authHeader = Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer "))
            return Ok(new {Message = "Invalid scheme"});

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            return Ok(new
            {
                Raw = token,
                jwt.Issuer,
                jwt.Audiences,
                Claims = jwt.Claims.Select(c => new { c.Type, c.Value }),
                Expiration = jwt.ValidTo
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to parse token: {ex.Message}");
        }/**/
        int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Response<object>? res = await _bankAccountService.GetAccountBalanceAsync(accountId);
        if (res.StatusMessage == "Success" && !String.IsNullOrWhiteSpace(res?.Data?.ToString()))
            return Ok(res);

        return Unauthorized(res);/**/
    }

    [Authorize]
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] TransactionRequest request)
    {
        int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Response<object> res = await _bankAccountService.WithdrawAsync(id, request.Amount);
        if (res.StatusMessage == "Success")
        {
            return Ok(res);
        }
        else if (res.StatusMessage == "Failed") return Conflict(res);

        return Unauthorized(res);
    }

    [Authorize]
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] TransactionRequest request)
    {
        int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Response<object> res = await _bankAccountService.DepositAsync(id, request.Amount);
        if (res.StatusMessage == "Success")
        {
            return Ok(res);
        }
        else if (res.StatusMessage == "Failed") return Conflict(res);

        return Unauthorized(res);
    }

}
