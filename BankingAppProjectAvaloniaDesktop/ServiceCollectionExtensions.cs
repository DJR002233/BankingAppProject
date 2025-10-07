using Microsoft.Extensions.DependencyInjection;
using BankingAppProjectAvaloniaDesktop.ViewModels;
using BankingAppProjectAvaloniaDesktop.Interfaces;
using BankingAppProjectAvaloniaDesktop.Services.CredentialManager;
using System;
using System.Runtime.InteropServices;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Services.Auth;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace BankingAppProjectAvaloniaDesktop;

public static class ServiceCollectionExtension
{
    public static IServiceCollection InitializeService(this IServiceCollection services, IConfiguration configuration)
    {
        // Register services
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            services.AddSingleton<ICredentialsStore, WindowsCredentialStore>();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services.AddSingleton<ICredentialsStore, LinuxSecretToolStore>();
        else
            throw new PlatformNotSupportedException();

        var baseUri = new Uri("https://localhost:7013/api/");

        services.AddTransient<NetworkErrorHandler>();
        services.AddTransient<AuthHeaderHandler>();
        //services.AddSingleton<Session>();
        services.AddSingleton<IAuthEventService, AuthEventService>();
        services.AddSingleton<ISessionManager, SessionManager>();
        services.AddSingleton<SessionManager>();
        services.AddTransient<SessionHeaderHandler>();

        // In DI setup
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = baseUri;
        }).AddHttpMessageHandler<SessionHeaderHandler>().AddHttpMessageHandler<NetworkErrorHandler>();

        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("ApiClient");
            var credentials = sp.GetRequiredService<ICredentialsStore>();
            return new SessionManager(client, credentials);
        });

        services.AddHttpClient<AuthService>(client =>
        {
            client.BaseAddress = baseUri;
        });
        services.AddHttpClient<BankAccountService>(client =>
        {
            client.BaseAddress = baseUri;
        }).AddHttpMessageHandler<AuthHeaderHandler>().AddHttpMessageHandler<NetworkErrorHandler>();
        
        services.AddScoped<NavigationService>();

        // Register viewmodels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<CreateAccountViewModel>();
        services.AddTransient<MainMenuViewModel>();
        services.AddTransient<TransactionFormViewModel>();
        services.AddTransient<TransactionOverviewViewModel>();
        services.AddTransient<TransactionResponseViewModel>();

        services.AddSingleton<Func<LoginViewModel>>(sp => () => sp.GetRequiredService<LoginViewModel>());
        services.AddSingleton<Func<CreateAccountViewModel>>(sp => () => sp.GetRequiredService<CreateAccountViewModel>());
        services.AddSingleton<Func<MainMenuViewModel>>(sp => () => sp.GetRequiredService<MainMenuViewModel>());
        services.AddSingleton<Func<string, decimal, TransactionFormViewModel>>(sp =>
            (transactionType, bankbalance) =>
                new TransactionFormViewModel(
                    transactionType,
                    bankbalance,
                    sp.GetRequiredService<NavigationService>(),
                    sp.GetRequiredService<Func<string, decimal, decimal, TransactionOverviewViewModel>>(),
                    sp.GetRequiredService<Func<MainMenuViewModel>>()));
        services.AddSingleton<Func<string, decimal, decimal, TransactionOverviewViewModel>>(sp =>
            (transactionType, transactionAmount, bankbalance) =>
                new TransactionOverviewViewModel(
                    transactionType,
                    transactionAmount,
                    bankbalance,
                    sp.GetRequiredService<NavigationService>(),
                    sp.GetRequiredService<Func<string, string, TransactionResponseViewModel>>(),
                    sp.GetRequiredService<Func<MainMenuViewModel>>(),
                    sp.GetRequiredService<BankAccountService>()));
        services.AddSingleton<Func<string, string, TransactionResponseViewModel>>(sp =>
            (transactionResponse, message) =>
                new TransactionResponseViewModel(
                    transactionResponse,
                    message,
                    sp.GetRequiredService<NavigationService>(),
                    sp.GetRequiredService<Func<MainMenuViewModel>>()));

        return services;
    }
}