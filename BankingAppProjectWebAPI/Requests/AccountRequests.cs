using System.ComponentModel.DataAnnotations;

namespace BankingAppProjectWebAPI.Requests;
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}

public class TokenRequest
{
    public string? Token { get; set; }
}

public class CreateAccountRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Name { get; set; }
}