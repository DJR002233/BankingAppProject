using System.Collections.Generic;

namespace BankingAppProjectAvaloniaDesktop.Models;

public class SimpleDialogModel<T>
{
    public string? Title { get; set; } = "";
    public int? Status { get; set; }
    public string? StatusMessage { get; set; } = "";
    public string? Message { get; set; } = "";
    public T? Data { get; set; }
}

public class SimpleDialogModel
{
    public string? Title { get; set; } = "";
    public int? Status { get; set; }
    public string? StatusMessage { get; set; } = "";
    public string? Message { get; set; } = "";
    public object? Data { get; set; }
}