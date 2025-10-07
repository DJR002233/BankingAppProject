using System.ComponentModel.DataAnnotations;

namespace BankingAppProjectWebAPI.Models;

public class BankAccountInformation
{
    [Key]
    public int? Id { get; set; }
    [Required, MaxLength(200)]
    public string? Name { get; set; }
    public decimal Money { get; set; } = 0;

    public int AccountId { get; set; }
    public AccountInformation Account { get; set; } = null!;

    public ICollection<TransactionLogs> Transactions { get; set; } = new List<TransactionLogs>();
}