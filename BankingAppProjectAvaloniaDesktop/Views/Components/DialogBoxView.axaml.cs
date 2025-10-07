using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BankingAppProjectAvaloniaDesktop.Views.Components;

public partial class DialogBoxView : Window
{
    public DialogBoxView()
    {
        InitializeComponent();
    }
    public DialogBoxView(string? title = "", string? message = "") :this()
    {
        this.Title = title;
        MessageText.Text = message;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
