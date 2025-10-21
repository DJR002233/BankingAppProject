using System;
using BankingAppProjectAvaloniaDesktop.Models;

namespace BankingAppProjectAvaloniaDesktop.Services;

public class ApiResponse
{
    public static SimpleDialogModel<T> Simplify<T>(ApiResponseModel<T>? res)
    {
        if (res is null)
            throw new ArgumentNullException(nameof(res), "API validation error response cannot be null");
        string message = res.Message ?? "";
        if (res.Errors is not null)
            foreach (var key in res.Errors)
            {
                string combined = string.Join(", ", key.Value ?? Array.Empty<string>());
                //message += $"{key.Key}: {combined}\n";
                message += $"{combined}\n";
            }
        return new SimpleDialogModel<T>
        {
            Title = res.Title,
            Status = res.Status,
            StatusMessage = res.StatusMessage,
            Message = message.Trim(),
            Data = res.Data
        };
    }

    public static SimpleDialogModel Simplify(ApiResponseModel? res)
    {
        if (res is null)
            throw new ArgumentNullException(nameof(res), "API validation error response cannot be null");
        string message = res.Message ?? "";
        if (res.Errors is not null)
            foreach (var key in res.Errors)
            {
                string combined = string.Join(", ", key.Value ?? Array.Empty<string>());
                //message += $"{key.Key}: {combined}\n";
                message += $"{combined}\n";
            }
        return new SimpleDialogModel
        {
            Title = res.Title,
            Status = res.Status,
            StatusMessage = res.StatusMessage,
            Message = message.Trim(),
            Data = res.Data
        };
    }

}