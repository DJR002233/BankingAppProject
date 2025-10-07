using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using BankingAppProjectAvaloniaDesktop.ViewModels;
using System.Globalization;
using System;
using Microsoft.Extensions.Configuration;

namespace BankingAppProjectAvaloniaDesktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-PH");
            CultureInfo.CurrentUICulture = new CultureInfo("en-PH");
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            // Use the extension method to wire up everything in one line
            services.InitializeService(configuration);

            // Register MainWindow
            services.AddTransient<MainWindow>(sp => new MainWindow
            {
                DataContext = sp.GetRequiredService<MainWindowViewModel>()
            });

            Services = services.BuildServiceProvider();

            desktop.MainWindow = Services.GetRequiredService<MainWindow>();

            //desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}