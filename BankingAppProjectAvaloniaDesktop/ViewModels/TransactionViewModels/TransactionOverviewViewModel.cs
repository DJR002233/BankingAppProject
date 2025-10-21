using System;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Commands;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Services;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class TransactionOverviewViewModel : ViewModelBase
{
    #region Services
    private readonly NavigationService _navigation;
    private readonly BankAccountService _bankAccount;
    #endregion Services

    #region VMs
    private readonly Func<MainMenuViewModel> _mainMenuVM;
    private readonly Func<string, string, TransactionResponseViewModel> _transactionResponseVM;
    #endregion VMs

    #region Commands
    public AsyncRelayCommand GoToTransactionResponseViewCommand { get; }
    public RelayCommand GoToMainMenuViewCommand { get; }
    #endregion Commands

    #region Fields
    private string _transactionType;
    private decimal _transactionAmount;
    private decimal _bankBalance;
    #endregion Fields

    #region Properties
    public string TransactionType
    {
        get => _transactionType;
        set => _transactionType = value;
    }
    public string TransactionAmount
    {
        get => $"{_transactionAmount:C}";
    }
    public string BankBalance
    {
        get => $"{_bankBalance:C}";
    }
    public string ResultBankBalance
    {
        get
        {
            string bankBalance;
            if (TransactionType == "Deposit")
                bankBalance = $"{_bankBalance + _transactionAmount:C}";
            else bankBalance = $"{_bankBalance - _transactionAmount:C}";
            return bankBalance;
        }
    }
    #endregion Properties

    public TransactionOverviewViewModel(string transactionType, decimal transactionAmount, decimal bankBalance, NavigationService navigation, Func<string, string, TransactionResponseViewModel> transactionResponseVM, Func<MainMenuViewModel> mainMenuVM, BankAccountService bankAccount)
    {
        _navigation = navigation;
        _transactionResponseVM = transactionResponseVM;
        _mainMenuVM = mainMenuVM;
        _transactionType = transactionType;
        _transactionAmount = transactionAmount;
        _bankBalance = bankBalance;
        _bankAccount = bankAccount;
        GoToMainMenuViewCommand = new RelayCommand(
            _ =>
            _navigation.NavigateTo(_mainMenuVM()));
        GoToTransactionResponseViewCommand = new AsyncRelayCommand(
            async _ => await GoToTransactionResponseView());
    }

    public async Task GoToTransactionResponseView()
    {
        SimpleDialogModel<object> res = new();
        if (_transactionType == "Withdraw")
            res = await _bankAccount.WithdrawAsync(_transactionAmount);
        else if (_transactionType == "Deposit")
            res = await _bankAccount.DepositAsync(_transactionAmount);

        string title = "", message = "";
        if (res.StatusMessage == "Success")
        {
            title = $"{TransactionType} Success!";
            message = $"{TransactionType} was successfully completed.";
        }
        else if (res.StatusMessage == "Failed")
        {
            title = $"{TransactionType} Failed!";
            message = res.Message!;
        }
        else
        {
            title = $"{res.Title} ({res.Status}): {res.StatusMessage}";
            message = res.Message!;
        }

        _navigation.NavigateTo(_transactionResponseVM(title, message));
    }
}