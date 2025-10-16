using System;
using System.Threading.Tasks;
using BankingAppProjectAvaloniaDesktop.Commands;
using BankingAppProjectAvaloniaDesktop.Models;
using BankingAppProjectAvaloniaDesktop.Services;
using BankingAppProjectAvaloniaDesktop.Services.Helper;
using BankingAppProjectAvaloniaDesktop.Servicesl;

namespace BankingAppProjectAvaloniaDesktop.ViewModels;

public class MainMenuViewModel : ViewModelBase
{
    #region Services
    private readonly NavigationService _navigation;
    private readonly BankAccountService _bankAccountService;
    public LoadingOverlay loadingOverlay { get; }
    #endregion Services

    #region VMs
    private readonly Func<LoginViewModel> _loginVM;
    private readonly Func<string, decimal, TransactionFormViewModel> _transactionFormVM;
    #endregion VMs

    #region Commands
    public AsyncRelayCommand GoToLoginViewCommand { get; }
    public AsyncRelayCommand<object> GoToTransactionFormViewCommand { get; }
    #endregion Commands

    #region Fields
    private decimal? _bankBalance;
    #endregion Fields

    #region Properties
    public decimal? BankBalance
    {
        get => _bankBalance;
        set
        {
            _bankBalance = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(BankBalanceCurrency));
        }
    }
    public string BankBalanceCurrency
    {
        get => $"{_bankBalance:C}";
    }

    #endregion Properties

    public MainMenuViewModel(NavigationService navigation, BankAccountService bankAccountService, Func<string, decimal, TransactionFormViewModel> transactionFormVM, Func<LoginViewModel> loginVM, LoadingOverlay loadingOverlay)
    {
        _navigation = navigation;
        _bankAccountService = bankAccountService;
        _transactionFormVM = transactionFormVM;
        _loginVM = loginVM;
        this.loadingOverlay = loadingOverlay;
        GoToLoginViewCommand = new AsyncRelayCommand(async _ => await GoToLoginView());
        GoToTransactionFormViewCommand = new AsyncRelayCommand<object>(GoToTransactionFormView);
    }

    public async Task GoToTransactionFormView(object? transactionType)
    {
        string tType = (string)transactionType!;
        if (tType == "Withdraw" && BankBalance <= 0)
        {
            await DialogBox.Show("Failed", "Insufficient Funds!");
            return;
        }
        _navigation.NavigateTo(_transactionFormVM(tType, _bankBalance ?? 0));
    }

    private async Task GoToLoginView()
    {
        SimpleDialogModel<object> res = await _bankAccountService.LogoutAsync();
        if (res.StatusMessage == "Success")
        {
            _navigation.NavigateTo(_loginVM());
            return;
        }
        else if (res.StatusMessage == "Failed")
        {
            await DialogBox.Show(res.StatusMessage, res.Message);
            _navigation.NavigateTo(_loginVM());
            return;
        }
        else if (res.StatusMessage == "Error")
        {
            await DialogBox.Show(res.Title, res.StatusMessage);
            _navigation.NavigateTo(_loginVM());
            return;
        }
        await DialogBox.Show("Error", "Unknown Error!");
        _navigation.NavigateTo(_loginVM());
    }

    public async Task<bool> InitializeAsync()
    {
        if (BankBalance is not null)
            return true;
        loadingOverlay.Show("Loading Bank Information...");
        SimpleDialogModel<object> bankBalance = await _bankAccountService.GetBankBalanceAsync();
        if (bankBalance.StatusMessage == "Success")
        {
            BankBalance = decimal.Parse(bankBalance.Data!.ToString()!);
            loadingOverlay.Close();
            return true;
        }
        //await _bankAccountService.LogoutAsync();
        // loadingOverlay.Close();
        await DialogBox.Show(bankBalance.StatusMessage, bankBalance.Message);
        _navigation.NavigateTo(_loginVM());
        return false;
    }

}