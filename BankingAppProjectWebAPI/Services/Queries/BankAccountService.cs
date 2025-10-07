using BankingAppProjectWebAPI.Data;
using BankingAppProjectWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingAppProjectWebAPI.Services.DependencyInjection;

public class BankAccountService
{
    private readonly MyDbContext _context;
    private PasswordHasher<AccountInformation> _passwordHasher = new PasswordHasher<AccountInformation>();

    public BankAccountService(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Response<object>> GetAccountBalanceAsync(int accountId)
    {
        decimal balance = await _context.BankAccounts.Where(b => b.AccountId == accountId)
        .Select(b => b.Money).FirstOrDefaultAsync();
        return new Response<object>() { StatusMessage = "Success", Data = balance };
    }

    public async Task<Response<object>> WithdrawAsync(int id, decimal amount)
    {
        var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.AccountId == id);
        if (bankAccount is null)
            return new Response<object> {StatusMessage = "Error", Message = "Bank account not found!\nPlease login again..."};

        bankAccount.Money -= amount;
        await _context.SaveChangesAsync();
        return new Response<object>() { StatusMessage = "Success" };
    }

    public async Task<Response<object>> DepositAsync(int id, decimal amount)
    {
        var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.AccountId == id);
        if (bankAccount is null)
            return new Response<object> {StatusMessage = "Error", Message = "Bank account not found!\nPlease login again..."};

        bankAccount.Money += amount;
        await _context.SaveChangesAsync();
        return new Response<object>() { StatusMessage = "Success" };
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
