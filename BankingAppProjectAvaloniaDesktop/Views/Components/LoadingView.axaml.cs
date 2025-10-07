using Avalonia;
using Avalonia.Controls;

namespace BankingAppProjectAvaloniaDesktop.Views.Components;

public partial class LoadingOverlay : UserControl
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<LoadingOverlay, bool>(nameof(IsActive));
    public static readonly StyledProperty<string> LoadingTextProperty =
        AvaloniaProperty.Register<LoadingOverlay, string>(nameof(LoadingText), "Loading...");

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public string LoadingText
    {
        get => GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    public LoadingOverlay()
    {
        InitializeComponent();
    }
}
