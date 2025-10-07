using CredentialManagement;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using BankingAppProjectAvaloniaDesktop.Interfaces;

namespace BankingAppProjectAvaloniaDesktop.Services.CredentialManager;

// use AdysTech.CredentialManager instead of CredentialManagement
// dotnet add package AdysTech.CredentialManager
public class WindowsCredentialStore : ICredentialsStore
{
    private const string Target = "BankingApp.AuthModel";

    public Task StoreAsync(string token)
    {
        //string json = JsonSerializer.Serialize(session);

        using var cred = new Credential()
        {
            Target = Target,
            Username = "BankingApp", // optional metadata
            Password = token,
            PersistanceType = PersistanceType.LocalComputer,
            Type = CredentialType.Generic
        };

        bool ok = cred.Save();
        if (!ok) throw new InvalidOperationException("Failed saving credential");
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync()
    {
        using var cred = new Credential { Target = Target, Type = CredentialType.Generic };
        if (!cred.Load())
            return Task.FromResult<string?>(null);

        try
        {
            return Task.FromResult(cred.Password ?? null);
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }

    public Task DeleteAsync()
    {
        using var cred = new Credential { Target = Target, Type = CredentialType.Generic };
        cred.Delete();
        return Task.CompletedTask;
    }
}

/*
using System.Net;
using CredentialManagement;
using System.Text.Json;
using System.Threading.Tasks;
using System;

public static class TokenStorage
{
    public static void StoreToken(string accessToken)
    {
        using (var cred = new ICredentials())
        {
            cred.Password = accessToken;
            cred.Target = "BankingApp.AccessToken";
            cred.Type = CredentialType.Generic;
            cred.PersistanceType = PersistanceType.LocalComputer;
            cred.Save();
        }
    }

    public static string? GetToken()
    {
        using (var cred = new Credential { Target = "BankingApp.AccessToken" })
        {
            return cred.Load() ? cred.Password : null;
        }
    }
}
*/