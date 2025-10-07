using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using BankingAppProjectAvaloniaDesktop.Views.Components;

namespace BankingAppProjectAvaloniaDesktop.Servicesl;

class DialogBox
{
    public static async Task Show(string? title, string? message)
    {
        var dialog = new DialogBoxView(title, message);

        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        await dialog.ShowDialog(lifetime!.MainWindow!);
    }
}