namespace BankingAppProjectAvaloniaDesktop.Services.Helper;

public class LoadingOverlay : ViewModelBase
{
    #region Fields
    private bool _isLoading;
    private string? _loadingMessage;
    #endregion Fields

    #region Property
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
    #endregion Property

    public LoadingOverlay()
    {
        _isLoading = false;
    }
    
    public void Show(string message)
    {
        LoadingMessage = message;
        IsLoading = true;
    }
    
    public void Close()
    {
        IsLoading = false;
        LoadingMessage = null;
    }

}