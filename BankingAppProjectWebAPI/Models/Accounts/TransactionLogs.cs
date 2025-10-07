using System.ComponentModel.DataAnnotations;

namespace BankingAppProjectWebAPI.Models;

public class TransactionLogs
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(200)]
    public required string Name { get; set; }
    [Required, MaxLength(200)]
    public required string TypeOfTransaction { get; set; }
    public required decimal Money { get; set; }
    public required DateTime DateTimeOfTransaction { get; set; }

    public required int BankAccountId { get; set; }
    public BankAccountInformation BankAccount { get; set; } = null!;
}