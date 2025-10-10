using Avalonia.Controls;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Commands;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Servicesl;
using System;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Services.Auth;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    #region Services
    private readonly NavigationService _navigation;
    private readonly AuthService _authService;
    private readonly SessionManager _sessionManager;
    #endregion Services
    #region VMs
    private readonly Func<CreateAccountViewModel> _createAccountVM;
    private readonly Func<MainMenuViewModel> _mainMenuVM;
    #endregion VMs
    #region Commands
    public RelayCommand GoToCreateAccountViewCommand { get; }
    public AsyncRelayCommand GoToMainMenuViewCommand { get; }
    #endregion Commands
    #region Fields
    private string? _email;
    private bool _isLoading;
    private string? _loadingMessage;
    #endregion Fields
    #region Properties
    public string? Email
    {
        get => _email;
        set => _email = value;
    }
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            RaisePropertyChanged();
        }
    }
    public string? LoadingMessage
    {
        get => _loadingMessage;
        set
        {
            _loadingMessage = value;
            RaisePropertyChanged();
        }
    }

    #endregion Properties
    public LoginViewModel(NavigationService navigation, AuthService authService, Func<CreateAccountViewModel> createAccountVM, Func<MainMenuViewModel> mainMenuVM, SessionManager sessionManager)
    {
        _navigation = navigation;
        _authService = authService;
        _createAccountVM = createAccountVM;
        _mainMenuVM = mainMenuVM;
        _sessionManager = sessionManager;
        IsLoading = false;
        GoToCreateAccountViewCommand = new RelayCommand(
            _ => _navigation.NavigateTo(_createAccountVM()));
        GoToMainMenuViewCommand = new AsyncRelayCommand(async PasswordBox =>
        {
            if (PasswordBox is not TextBox passwordBox)
                return;

            if (Email is null || passwordBox.Text is null)
            {
                await DialogBox.Show("Alert", "Email and Password are required!");
                return;
            }

            IsLoading = true;
            LoadingMessage = "Logging in...";
            SimpleDialogModel<AuthModel> res = await _authService.LoginAsync(Email, passwordBox.Text);
            if (res.StatusMessage == "Success")
            {
                passwordBox.Text = string.Empty;
                MainMenuViewModel mainmenuVM = _mainMenuVM();
                bool initialized = await mainmenuVM.InitializeAsync();
                IsLoading = false;
                LoadingMessage = null;
                if (initialized)
                    _navigation.NavigateTo(mainmenuVM);
                return;
            }
            else if (res.StatusMessage == "Failed" || res.Status is not null)
            {
                IsLoading = false;
                LoadingMessage = null;
                await DialogBox.Show(res.StatusMessage ?? "", res.Message ?? "");
                return;
            }
            else if (res.StatusMessage == "Error")
            {
                IsLoading = false;
                LoadingMessage = null;
                await DialogBox.Show(res.Title ?? "", res.Message ?? "");
                return;
            }
            IsLoading = false;
            LoadingMessage = null;
            await DialogBox.Show("Error", "Unknown Error!");
        });
    }
    public async Task InitializeAsync()
    {
        IsLoading = true;
        LoadingMessage = "Checking Session...";
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
        IsLoading = false;
        LoadingMessage = null;
    }
}