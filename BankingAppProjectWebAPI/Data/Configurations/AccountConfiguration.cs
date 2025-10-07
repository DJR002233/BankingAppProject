using BankingAppProjectWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAppProjectWebAPI.Data.Configurations;

public static class AccountConfiguration
{
    public static void ConfigureAccount(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountInformation>(entity =>
        {
            entity.ToTable("AccountInformation");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Email).IsRequired().HasMaxLength(254).HasColumnType("varchar");
            entity.Property(b => b.Password).IsRequired().HasColumnType("bytea");
            //entity.Property(b => b.Token).HasColumnType("Text");
        });
    }
}