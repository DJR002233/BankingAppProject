using Avalonia.Controls;

namespace BankingAppProjectAvaloniaDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var screen = Screens.Primary;
        if (screen != null)
        {
            const double scale = 0.4;
            var workingArea = screen.WorkingArea;
            this.Width = workingArea.Width * scale;
            this.Height = workingArea.Height * scale;
        }
    }
}