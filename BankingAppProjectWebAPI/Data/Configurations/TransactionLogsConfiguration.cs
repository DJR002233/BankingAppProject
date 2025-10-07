using BankingAppProjectWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAppProjectWebAPI.Data.Configurations;

public static class TransactionLogsConfiguration
{
    public static void ConfigureTransactionLogs(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionLogs>(entity =>
        {
            entity.ToTable("TransactionLogs");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
            entity.Property(b => b.TypeOfTransaction).IsRequired().HasMaxLength(30);
            entity.Property(b => b.Money).IsRequired();
            entity.Property(b => b.DateTimeOfTransaction).IsRequired().HasDefaultValueSql("NOW()");

            entity.HasOne(b => b.BankAccount)
                .WithMany(b => b.Transactions)
                .HasForeignKey(b => b.BankAccountId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
    
}