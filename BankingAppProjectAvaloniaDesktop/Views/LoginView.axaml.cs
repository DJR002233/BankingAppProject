
using System.Threading;
using Avalonia.Controls;
using BankingAppProjectAvaloniaDesktop.ViewModels;

namespace BankingAppProjectAvaloniaDesktop.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += async (_, __) =>
        {
            IsEnabled = false;
            try
            {
                if (DataContext is LoginViewModel vm)
                    await vm.InitializeAsync();
            }
            finally
            {
                IsEnabled = true;
            }
        };
    }
}