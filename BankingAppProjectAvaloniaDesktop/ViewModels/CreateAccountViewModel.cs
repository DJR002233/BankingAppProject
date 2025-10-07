using System;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Commands;
using BankingAppProjectAvaloniaDesktop.Services;
using System.Security;
using BankingAppProjectAvaloniaDesktop.Interfaces;
using BankingAppProjectAvaloniaDesktop.Servicesl;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class CreateAccountViewModel : ViewModelBase
{
    #region Services
    private readonly NavigationService _navigation;
    private readonly AuthService _authService;
    #endregion Services
    #region VMs
    private readonly Func<LoginViewModel> _loginVM;
    #endregion VMs
    #region Commands
    public RelayCommand GoToLoginViewCommand { get; }
    public AsyncRelayCommand CreateAccountCommand { get; }
    #endregion Commands
    #region Fields
    private string? _email;
    private string? _name;
    #endregion Fields
    #region Properties
    public string? Email
    {
        get => _email;
        set => _email = value;
    }
    public string? Name
    {
        get => _name;
        set => _name = value;
    }
    #endregion Properties
    public CreateAccountViewModel(NavigationService navigation, AuthService authService, Func<LoginViewModel> loginVM)
    {
        _navigation = navigation;
        _authService = authService;
        _loginVM = loginVM;
        CreateAccountCommand = new AsyncRelayCommand(async passwords =>
        {
            if (passwords is not Passwords passwordBoxes)
                return;

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email))
            {
                await DialogBox.Show("Alert!", "Name and Email are required!");
                return;
            }

            if (passwordBoxes.Password != passwordBoxes.RePassword)
            {
                await DialogBox.Show("Alert!", "Password does not match!");
                return;
            }

            //Console.WriteLine(passwordBoxes.Password);
            //Console.WriteLine(passwordBoxes.RePassword);

            SimpleDialogModel<object> res = await _authService.CreateAccountAsync(Name, Email, passwordBoxes.Password);

            if (res.StatusMessage == "Success")
            {
                passwordBoxes.Password = string.Empty;
                passwordBoxes.RePassword = string.Empty;
                string message = res.Message ?? "";
                message += "\nYou will be redirected back to the login page";
                await DialogBox.Show(res.StatusMessage, message);
                _navigation.NavigateTo(_loginVM());
                return;
            }
            else if (res.StatusMessage == "Failed" || res.Status is not null)
            {
                await DialogBox.Show(res.StatusMessage ?? "", res.Message ?? "");
                return;
            }

            await DialogBox.Show("Error", "Unknown Error!");
            // Console.WriteLine(res.Title);
            // Console.WriteLine(res.Status);
            // Console.WriteLine(res.StatusMessage);
            // Console.WriteLine(res.Message);
        });
        GoToLoginViewCommand = new RelayCommand(
            _ => _navigation.NavigateTo(_loginVM()));
    }
}