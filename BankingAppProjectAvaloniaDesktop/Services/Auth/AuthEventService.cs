using System;

namespace BankingAppProjectAvaloniaDesktop.Services.Auth;

public interface IAuthEventService
{
    event Action? OnUnauthorized;
    void TriggerUnauthorized();
}

public class AuthEventService : IAuthEventService
{
    public event Action? OnUnauthorized;

    public void TriggerUnauthorized() => OnUnauthorized?.Invoke();
}
