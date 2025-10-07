using System;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Services.Auth;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public NavigationService Navigation { get; }
    private readonly Func<LoginViewModel> _loginViewModel;
    private readonly IAuthEventService _authEvent;
    public MainWindowViewModel(NavigationService navigation, Func<LoginViewModel> loginViewModel, IAuthEventService authEvent)
    {
        Navigation = navigation;
        _loginViewModel = loginViewModel;
        _authEvent = authEvent;
        _authEvent.OnUnauthorized += HandleUnauthorized;
        Navigation.NavigateTo(_loginViewModel());
    }
    private void HandleUnauthorized()
    {
        Navigation.NavigateTo(_loginViewModel());
    }
}