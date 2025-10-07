using System;
using BankingAppProjectAvaloniaDesktop.Commands;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Servicesl;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class TransactionFormViewModel : ViewModelBase
{
    #region Services
    private readonly NavigationService _navigation;
    #endregion Services
    #region VMs
    private readonly Func<string, decimal, decimal, TransactionOverviewViewModel> _transactionOverviewVM;
    private readonly Func<MainMenuViewModel> _mainMenuVM;
    #endregion VMs
    #region Commands
    public RelayCommand GoToMainMenuViewCommand { get; }
    public AsyncRelayCommand GoToTransactionOverviewViewCommand { get; }
    #endregion Commands
    #region Fields
    private string _transactionType;
    private decimal? _transactionAmount;
    private decimal _bankBalance;
    #endregion Fields
    #region Properties
    public string TransactionType
    {
        get => _transactionType;
        set => _transactionType = value;
    }
    public decimal? TransactionAmount
    {
        get => _transactionAmount;
        set => _transactionAmount = value;
    }
    #endregion Properties
    public TransactionFormViewModel(string transactionType, decimal bankBalance, NavigationService navigation, Func<string, decimal, decimal, TransactionOverviewViewModel> transactionOverviewVM, Func<MainMenuViewModel> mainMenuVM)
    {
        _navigation = navigation;
        _transactionOverviewVM = transactionOverviewVM;
        _mainMenuVM = mainMenuVM;
        _transactionType = transactionType;
        _bankBalance = bankBalance;
        GoToTransactionOverviewViewCommand = new AsyncRelayCommand(
            async _ =>
            {
                if (transactionType == "Withdraw" && TransactionAmount > _bankBalance)
                {
                    await DialogBox.Show("Failed", "Insufficient Funds!");
                    return;
                }
                _navigation.NavigateTo(_transactionOverviewVM(TransactionType, TransactionAmount ?? 0.00m, _bankBalance));
            });
        GoToMainMenuViewCommand = new RelayCommand(
            _ => _navigation.NavigateTo(_mainMenuVM()));
    }
}