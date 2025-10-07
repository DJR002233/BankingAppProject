using BankingAppProjectWebAPI.Data.Configurations;
using BankingAppProjectWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAppProjectWebAPI.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    // Tables
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("main");
        modelBuilder.ConfigureAccount();
        modelBuilder.ConfigureBankAccount();
        modelBuilder.ConfigureToken();
        modelBuilder.ConfigureTransactionLogs();
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<AccountInformation> Accounts { get; set; }
    public DbSet<BankAccountInformation> BankAccounts { get; set; }
    public DbSet<Tokens> Tokens { get; set; }
    public DbSet<TransactionLogs> TransactionLogs { get; set; }
}