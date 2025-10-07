
using Avalonia.Controls;
using BankingAppProjectAvaloniaDesktop.ViewModels;

namespace BankingAppProjectAvaloniaDesktop.Views;

public partial class MainMenuView : UserControl
{
    public MainMenuView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += async (_, __) =>
        {
            IsEnabled = false;
            try
            {
                if (DataContext is MainMenuViewModel vm)
                    await vm.InitializeAsync();
            }
            finally
            {
                IsEnabled = true;
            }
        };
    }
}