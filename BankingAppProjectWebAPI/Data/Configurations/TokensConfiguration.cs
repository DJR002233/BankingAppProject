using BankingAppProjectWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAppProjectWebAPI.Data.Configurations;

public static class TokenConfiguration
{
    public static void ConfigureToken(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tokens>(entity =>
        {
            entity.ToTable("Tokens");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Token).HasColumnType("Text");
            entity.Property(b => b.ExpiresAt);
            entity.Property(b => b.Revoked);
            entity.Property(b => b.CreatedAt);
            entity.Property(b => b.ReplacedByToken);

            entity.HasOne(b => b.Account)
                .WithMany(b => b.Token)
                .HasForeignKey(b => b.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property<uint>("xmin").IsRowVersion().IsConcurrencyToken();
        });
    }
}