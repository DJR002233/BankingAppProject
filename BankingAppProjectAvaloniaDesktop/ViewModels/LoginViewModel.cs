using Avalonia.Controls;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Commands;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Servicesl;
using System;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Services.Auth;
using BankingAppProjectAvaloniaDesktop.Services.Helper;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    #region Services
    private readonly NavigationService _navigation;
    private readonly AuthService _authService;
    private readonly SessionManager _sessionManager;
    public LoadingOverlay loadingOverlay { get; }
    #endregion Services

    #region VMs
    private readonly Func<CreateAccountViewModel> _createAccountVM;
    private readonly Func<MainMenuViewModel> _mainMenuVM;
    #endregion VMs

    #region Commands
    public RelayCommand GoToCreateAccountViewCommand { get; }
    public AsyncRelayCommand<object> GoToMainMenuViewCommand { get; }
    #endregion Commands

    #region Fields
    private string? _email;
    #endregion Fields

    #region Properties
    public string? Email
    {
        get => _email;
        set => _email = value;
    }

    #endregion Properties

    public LoginViewModel(NavigationService navigation, AuthService authService, Func<CreateAccountViewModel> createAccountVM, Func<MainMenuViewModel> mainMenuVM, SessionManager sessionManager, LoadingOverlay loadingOverlay)
    {
        _navigation = navigation;
        _authService = authService;
        _createAccountVM = createAccountVM;
        _mainMenuVM = mainMenuVM;
        _sessionManager = sessionManager;
        this.loadingOverlay = loadingOverlay;
        GoToCreateAccountViewCommand = new RelayCommand(
            _ => _navigation.NavigateTo(_createAccountVM()));
        GoToMainMenuViewCommand = new AsyncRelayCommand<object>(LoginAsync);
    }

    public async Task LoginAsync(object? passwordBox)
    {
        if (passwordBox is not TextBox password)
            return;

        if (Email is null || password.Text is null)
        {
            await DialogBox.Show("Alert", "Email and Password are required!");
            return;
        }

        loadingOverlay.Show("Logging in...");
        SimpleDialogModel<AuthModel> res = await _authService.LoginAsync(Email, password.Text);
        if (res.StatusMessage == "Success")
        {
            password.Text = string.Empty;
            MainMenuViewModel mainmenuVM = _mainMenuVM();
            bool initialized = await mainmenuVM.InitializeAsync();
            if (initialized)
                _navigation.NavigateTo(mainmenuVM);
            return;
        }
        else if (res.StatusMessage == "Failed" || res.Status is not null)
        {
            loadingOverlay.Close();
            await DialogBox.Show(res.StatusMessage ?? "", res.Message ?? "");
            return;
        }
        else if (res.StatusMessage == "Error")
        {
            loadingOverlay.Close();
            await DialogBox.Show(res.Title ?? "", res.Message ?? "");
            return;
        }
        //loadingOverlay.Close();
        await DialogBox.Show("Error", "Unknown Error!");
    }

    public async Task InitializeAsync()
    {
        loadingOverlay.Show("Checking Session...");
        await _sessionManager.InitializeSession();
        SimpleDialogModel<string>? res = await _sessionManager.GetAccessTokenAsync();
        if (res.StatusMessage == "Success" && !String.IsNullOrWhiteSpace(res.Data))
        {
            MainMenuViewModel mainmenuVM = _mainMenuVM();
            bool initialized = await mainmenuVM.InitializeAsync();
            if (initialized)
                _navigation.NavigateTo(mainmenuVM);
        }
        else if (res.StatusMessage == "Failed")
        {
            await DialogBox.Show(res.StatusMessage, res.Message);
        }
        else if (res.StatusMessage == "Error")
        {
            await DialogBox.Show(res.Title, res.Message);
        }
        loadingOverlay.Close();
    }

}