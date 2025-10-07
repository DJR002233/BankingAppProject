using BankingAppProjectWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAppProjectWebAPI.Data.Configurations;

public static class BankAccountConfiguration
{
    public static void ConfigureBankAccount(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccountInformation>(entity =>
        {
            entity.ToTable("BankAccountInformation");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Money).IsRequired().HasDefaultValue(0);

            entity.HasOne(b => b.Account)
                .WithOne(b => b.BankAccount)
                .HasForeignKey<BankAccountInformation>(b => b.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}