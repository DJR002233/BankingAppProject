using System;
using BankingAppProjectAvaloniaDesktop.Commands;
using BankingAppProjectAvaloniaDesktop.Services;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class TransactionResponseViewModel : ViewModelBase
{
    #region Services
    private readonly NavigationService _navigation;
    #endregion Services

    #region VMs
    private readonly Func<MainMenuViewModel> _mainMenuVM;
    #endregion VMs

    #region Commands
    public RelayCommand GoToMainMenuViewCommand { get; }
    #endregion Commands

    #region Fields
    private string _transactionResponse;
    private string _message;
    #endregion Fields

    #region Properties
    public string TransactionResponse
    {
        get => _transactionResponse;
    }
    public string Message
    {
        get => _message;
    }
    #endregion Properties

    public TransactionResponseViewModel(string transactionResponse, string message, NavigationService navigation, Func<MainMenuViewModel> mainMenuVM)
    {
        _navigation = navigation;
        _mainMenuVM = mainMenuVM;
        _transactionResponse = transactionResponse;
        _message = message;
        GoToMainMenuViewCommand = new RelayCommand(
            _ =>
            _navigation.NavigateTo(_mainMenuVM()));
    }
    
}