using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BankingAppProjectAvaloniaDesktop.Interfaces;

namespace BankingAppProjectAvaloniaDesktop.Services.CredentialManager;

public class LinuxSecretToolStore : ICredentialsStore
{
    private const string AttrLabel = "RefreshToken";
    private const string AttrApp = "BankingApp";
    private const string AttrType = "Auth-Token";

    private static bool IsSecretToolAvailable()
    {
        try
        {
            var p = Process.Start(new ProcessStartInfo("which", "secret-tool")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            p?.WaitForExit();
            return p?.ExitCode == 0;
        }
        catch { return false; }
    }

    public async Task StoreAsync(string token)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            throw new PlatformNotSupportedException();

        if (!IsSecretToolAvailable())
            throw new InvalidOperationException("secret-tool is not installed on this machine.");

        //string json = JsonSerializer.Serialize(session.RefreshToken);

        // secret-tool store app <app> type <type>
        var psi = new ProcessStartInfo("secret-tool",
            $"store --label='{AttrLabel}' app {AttrApp} type {AttrType}")
        {
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var p = Process.Start(psi) ?? throw new InvalidOperationException("failed to start secret-tool");
        await p.StandardInput.WriteAsync(token);
        p.StandardInput.Close();
        await p.WaitForExitAsync();
        if (p.ExitCode != 0)
            throw new InvalidOperationException($"secret-tool store failed with exit code {p.ExitCode}");
    }

    public async Task<string?> GetAsync()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            throw new PlatformNotSupportedException();

        if (!IsSecretToolAvailable())
            return null;

        var psi = new ProcessStartInfo("secret-tool",
            $"lookup app {AttrApp}")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var p = Process.Start(psi) ?? throw new InvalidOperationException("failed to start secret-tool");
        string output = await p.StandardOutput.ReadToEndAsync();
        await p.WaitForExitAsync();
        if (p.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
            return null;

        try
        {
            return output.Trim();
        }
        catch
        {
            return null;
        }
    }

    public async Task DeleteAsync()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            throw new PlatformNotSupportedException();

        if (!IsSecretToolAvailable())
            return;

        var psi = new ProcessStartInfo("secret-tool",
            $"clear app {AttrApp}")
        {
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var p = Process.Start(psi) ?? throw new InvalidOperationException("failed to start secret-tool");
        await p.WaitForExitAsync();
    }
}
