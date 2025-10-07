namespace BankingAppProjectWebAPI.Models;

public class JwtTokenModel
{
    public required string Token { get; set; }
    public required DateTime Expiration { get; set;}
}