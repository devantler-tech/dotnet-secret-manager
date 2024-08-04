using System.Runtime.InteropServices;

namespace Devantler.SOPSManager.Core.Age;

static class SopsAgeKeyFileWriter
{
  internal static async Task AddKeyAsync(string key, CancellationToken token = default)
  {
    string sopsAgeKeyFile = Environment.GetEnvironmentVariable("SOPS_AGE_KEY_FILE") ?? "";
    if (!string.IsNullOrWhiteSpace(sopsAgeKeyFile))
      await WriteKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      sopsAgeKeyFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/sops/age/keys.txt";
      await WriteKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);

    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      string xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config";
      sopsAgeKeyFile = $"{xdgConfigHome}/sops/age/keys.txt";
      await WriteKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);

    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      sopsAgeKeyFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/sops/age/keys.txt";
      await WriteKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);
    }
  }

  internal static async Task RemoveKeyAsync(string key, CancellationToken token = default)
  {
    string sopsAgeKeyFile = Environment.GetEnvironmentVariable("SOPS_AGE_KEY_FILE") ?? "";
    if (!string.IsNullOrWhiteSpace(sopsAgeKeyFile))
      await RemoveKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      sopsAgeKeyFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/sops/age/keys.txt";
      await RemoveKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      string xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config";
      sopsAgeKeyFile = $"{xdgConfigHome}/sops/age/keys.txt";
      await RemoveKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      sopsAgeKeyFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/sops/age/keys.txt";
      await RemoveKeyAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);
    }
  }

  static async Task RemoveKeyAsync(string sopsAgeKeyFile, string key, CancellationToken token)
  {
    if (!string.IsNullOrWhiteSpace(sopsAgeKeyFile) && File.Exists(sopsAgeKeyFile))
    {
      string fileContents = await File.ReadAllTextAsync(sopsAgeKeyFile, token).ConfigureAwait(false);
      if (fileContents.Contains(key, StringComparison.Ordinal))
      {
        fileContents = fileContents.Replace(key, "", StringComparison.Ordinal);
        await File.WriteAllTextAsync(sopsAgeKeyFile, fileContents, token).ConfigureAwait(false);
      }
    }
  }

  internal static async Task<string> ReadFileAsync(CancellationToken token = default)
  {
    string sopsAgeKeyFileContents = "";
    string sopsAgeKeyFile = Environment.GetEnvironmentVariable("SOPS_AGE_KEY_FILE") ?? "";
    if (!string.IsNullOrWhiteSpace(sopsAgeKeyFile))
      sopsAgeKeyFileContents = await File.ReadAllTextAsync(sopsAgeKeyFile, token).ConfigureAwait(false);
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      sopsAgeKeyFileContents = await File.ReadAllTextAsync($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/sops/age/keys.txt", token).ConfigureAwait(false);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      string xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config";
      sopsAgeKeyFileContents = await File.ReadAllTextAsync($"{xdgConfigHome}/sops/age/keys.txt", token).ConfigureAwait(false);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      sopsAgeKeyFileContents = await File.ReadAllTextAsync($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/sops/age/keys.txt", token).ConfigureAwait(false);
    }

    return sopsAgeKeyFileContents;
  }

  static async Task WriteKeyAsync(string sopsAgeKeyFile, string key, CancellationToken token)
  {
    if (!string.IsNullOrWhiteSpace(sopsAgeKeyFile))
    {
      string? directory = Path.GetDirectoryName(sopsAgeKeyFile);
      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        _ = Directory.CreateDirectory(directory);
      if (!File.Exists(sopsAgeKeyFile))
      {
        using var fs = File.Create(sopsAgeKeyFile);
      }
      string fileContents = await File.ReadAllTextAsync(sopsAgeKeyFile, token).ConfigureAwait(false);
      if (!fileContents.Contains(key, StringComparison.Ordinal))
        await File.AppendAllTextAsync(sopsAgeKeyFile, key, token).ConfigureAwait(false);
    }
  }
}

