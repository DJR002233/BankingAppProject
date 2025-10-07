using System.ComponentModel.DataAnnotations;

namespace BankingAppProjectWebAPI.Models;

public class AccountInformation
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public BankAccountInformation BankAccount { get; set; } = null!;
    public ICollection<Tokens> Token { get; set; } = new List<Tokens>();
}