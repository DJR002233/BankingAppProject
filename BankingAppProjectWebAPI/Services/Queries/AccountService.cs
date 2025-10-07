using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BankingAppProjectWebAPI.Data;
using BankingAppProjectWebAPI.Models;
using BankingAppProjectWebAPI.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BankingAppProjectWebAPI.Services.DependencyInjection;

public class AccountService
{
    private readonly MyDbContext _context;
    private IConfiguration _configuration;
    private PasswordHasher<AccountInformation> _passwordHasher;

    public AccountService(MyDbContext context, IConfiguration configuration, PasswordHasher<AccountInformation> passwordHasher)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<Response<object>> LoginAccountAsync(LoginRequest user)
    {
        var invalidResponse = new Response<object>() { StatusMessage = "Failed", Message = "Invalid Credentials!" };

        var account = await _context.Accounts.FirstOrDefaultAsync(b => b.Email == user.Email);
        if (account is null)
            return invalidResponse;

        var passwordResult = _passwordHasher.VerifyHashedPassword(new AccountInformation { Email = user.Email, Password = user.Password }, account.Password!, user.Password);
        if (passwordResult != PasswordVerificationResult.Success)
            return invalidResponse;

        string dbToken = GenerateRefreshToken();
        JwtTokenModel jwtToken = GenerateAccessToken(account.Id, account.Email);

        var newToken = new Tokens
        {
            Token = dbToken,
            AccountId = account.Id,
        };
        account.Token.Add(newToken);
        await _context.SaveChangesAsync();

        return new Response<object>()
        {
            StatusMessage = "Success",
            Data = new
            {
                accessToken = jwtToken.Token,
                expiration = jwtToken.Expiration,
                refreshToken = dbToken
            }
        };
    }

    public async Task<Response<object>> CreateAccountAsync(AccountInformation user)
    {
        user.Password = _passwordHasher.HashPassword(user, user.Password!);
        bool emailExists = await _context.Accounts.AnyAsync(b => b.Email == user.Email);
        if (emailExists)
            return new Response<object>() { StatusMessage = "Failed", Message = "Email is already taken!" };
        _context.Accounts.Add(user);
        int rowsAffected = await _context.SaveChangesAsync();
        return new Response<object>() { StatusMessage = "Success", Message = "Account Created!" };
    }

    public async Task<Response<object>> GetAccessTokenAsync(string? rfToken)
    {
        if (rfToken == null)
            return new Response<object>() { StatusMessage = "Failed", Message = "No Token!" };

        var token = await _context.Tokens.Include(t => t.Account).FirstOrDefaultAsync(t => t.Token == rfToken);
        if (token is null || token.Revoked)
            return new Response<object>() { StatusMessage = "Failed", Message = "Invalid Token!\nPlease login again..." };

        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            token.Revoked = true;
            await _context.SaveChangesAsync();
            return new Response<object>() { StatusMessage = "Failed", Message = "Token Expired!\nPlease login again..." };
        }

        string refreshToken = GenerateRefreshToken();

        token.Revoked = true;
        token.ReplacedByToken = refreshToken;
        _context.Tokens.Add(new Tokens { Token = refreshToken, AccountId = token.AccountId });

        JwtTokenModel accessToken = GenerateAccessToken(token.AccountId, token.Account.Email);

        await _context.SaveChangesAsync();

        return new Response<object>()
        {
            StatusMessage = "Success",
            Data = new
            {
                accessToken = accessToken.Token,
                expiration = accessToken.Expiration,
                refreshToken
            }
        };
    }

    public async Task<Response<object>> ClearTokenAsync(string? rfToken)
    {
        if (rfToken == null)
            return new Response<object>() { StatusMessage = "Failed", Message = "No Token!" };

        var token = await _context.Tokens.Include(t => t.Account).FirstOrDefaultAsync(t => t.Token == rfToken);
        if (token is null || token.Revoked)
            return new Response<object>() { StatusMessage = "Failed", Message = "Invalid Token!\nPlease login again..." };

        token.Revoked = true;

        await _context.SaveChangesAsync();

        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            return new Response<object>() { StatusMessage = "Failed", Message = "Token Expired!\nPlease login again..." };
        }

        return new Response<object>()
        {
            StatusMessage = "Success"
        };
    }

    public JwtTokenModel GenerateAccessToken(int id, string email)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", "User")
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var tokenExpiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtToken = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: tokenExpiration,
            signingCredentials: creds
        );
        return new JwtTokenModel
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            Expiration = tokenExpiration
        };
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        string token = Convert.ToBase64String(bytes);
        return token;
    }

    /*
        // Read
        public async Task<List<AccountInformation>> GetAccountAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        // Update
        public async Task<bool> UpdateAccountAsync(AccountInformation user)
        {
            _context.Accounts.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        // Delete
        public async Task<bool> DeleteAccountAsync(int id)
        {
            var user = await _context.Accounts.FindAsync(id);
            if (user == null) return false;

            _context.Accounts.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
    */

}
