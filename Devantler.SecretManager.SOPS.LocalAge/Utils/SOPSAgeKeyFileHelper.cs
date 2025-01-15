using System.Runtime.InteropServices;

namespace Devantler.SecretManager.SOPS.LocalAge.Utils;


/// <summary>
/// Helper class to access and manipulate the SOPS Age key file.
/// </summary>
public class SOPSAgeKeyFileHelper
{

  /// <summary>
  /// Get the path to the SOPS_AGE_KEY_FILE or the default path for the current OS.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  public static string GetSOPSAgeKeyFilePath()
  {
    string? sopsAgeKeyFileEnvironmentVariable = Environment.GetEnvironmentVariable("SOPS_AGE_KEY_FILE");
    string sopsAgeKeyFilePath = "";
    if (!string.IsNullOrWhiteSpace(sopsAgeKeyFileEnvironmentVariable))
    {
      if (!File.Exists(sopsAgeKeyFileEnvironmentVariable))
        throw new ArgumentException($"The SOPS_AGE_KEY_FILE environment variable points to a file that does not exist: {sopsAgeKeyFileEnvironmentVariable}");
      sopsAgeKeyFilePath = sopsAgeKeyFileEnvironmentVariable;
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      sopsAgeKeyFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/sops/age/keys.txt";

    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      string xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config";
      sopsAgeKeyFilePath = $"{xdgConfigHome}/sops/age/keys.txt";

    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      sopsAgeKeyFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/sops/age/keys.txt";
    }
    return sopsAgeKeyFilePath;
  }
}
