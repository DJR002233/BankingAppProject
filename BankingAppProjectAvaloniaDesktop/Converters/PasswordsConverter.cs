using System;
using System.Collections.Generic;
using Avalonia.Data.Converters;
using BankingAppProjectAvaloniaDesktop.Models;

namespace BankingAppProjectAvaloniaDesktop.Converters;
public class PasswordsConverter : IMultiValueConverter
{
    public PasswordsConverter() { }
    public object Convert(IList<object?> values, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return new Passwords()
        {
            Password = values[0]?.ToString() ?? string.Empty,
            RePassword = values[1]?.ToString() ?? string.Empty
        };
    }
}