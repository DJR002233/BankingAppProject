using System.Collections.Generic;

namespace BankingAppProjectAvaloniaDesktop.Models;

public class ApiResponseModel<T>
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? StatusMessage { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]> Errors { get; set; } = new();
    public string? TraceId { get; set; }
}