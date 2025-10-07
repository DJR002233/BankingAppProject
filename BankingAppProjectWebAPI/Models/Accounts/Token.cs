namespace BankingAppProjectWebAPI.Models;

public class Tokens
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
    //public bool Used { get; set; } = false;
    public bool Revoked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ReplacedByToken { get; set; }

    public int AccountId { get; set; }
    public AccountInformation Account { get; set; } = null!;
    //public byte[] RowVersion { get; set; } = null!;

}