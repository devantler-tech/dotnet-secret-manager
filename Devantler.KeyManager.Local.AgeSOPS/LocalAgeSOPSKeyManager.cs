using Devantler.Keys.Age;
using Devantler.KeyManager.Core;
using Devantler.AgeCLI;
using System.Runtime.InteropServices;

namespace Devantler.KeyManager.Local.AgeSOPS;

/// <summary>
/// A local key manager for SOPS with Age keys.
/// </summary>
public class LocalAgeSOPSKeyManager() : IKeyManager<AgeKey>
{
  readonly string _sopsAgeKeyFile = GetSOPSAgeKeyFilePath();

  /// <inheritdoc/>
  public async Task<AgeKey> CreateKeyAsync(string? outKeyPath = default, CancellationToken token = default)
  {
    // Create a new Age key.
    var key = await AgeKeygen.InMemory(token).ConfigureAwait(false);

    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(outKeyPath))
      outKeyPath = _sopsAgeKeyFile;

    // Create the directory if it does not exist.
    string? directory = Path.GetDirectoryName(outKeyPath);
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      _ = Directory.CreateDirectory(directory);

    // Create the file if it does not exist.
    if (!File.Exists(outKeyPath))
    {
      using var fs = File.Create(outKeyPath);
    }

    // Append the key to the file if it does not exist.
    string fileContents = await File.ReadAllTextAsync(outKeyPath, token).ConfigureAwait(false);
    if (!fileContents.Contains(key.ToString(), StringComparison.Ordinal))
      await File.AppendAllTextAsync(outKeyPath, key.ToString() + Environment.NewLine, token).ConfigureAwait(false);

    return key;
  }

  /// <inheritdoc/>
  public async Task<AgeKey> DeleteKeyAsync(AgeKey key, string? keyPath = null, CancellationToken token = default)
  {
    ArgumentNullException.ThrowIfNull(key);

    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(keyPath))
      keyPath = _sopsAgeKeyFile;

    // Delete the key from the file.
    string fileContents = await File.ReadAllTextAsync(keyPath, token).ConfigureAwait(false);
    if (fileContents.Contains(key.ToString(), StringComparison.Ordinal))
    {
      fileContents = fileContents.Replace(key.ToString(), "", StringComparison.Ordinal);
      if (fileContents.EndsWith(Environment.NewLine, StringComparison.Ordinal))
        fileContents = fileContents[..^Environment.NewLine.Length];
      await File.WriteAllTextAsync(keyPath, fileContents, token).ConfigureAwait(false);
    }

    return key;
  }

  /// <inheritdoc/>
  public async Task<AgeKey> DeleteKeyAsync(string publicKey, string? keyPath = null, CancellationToken token = default)
  {
    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(keyPath))
      keyPath = _sopsAgeKeyFile;

    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(keyPath, token).ConfigureAwait(false);

    // Find the line number with the public key
    string[] lines = fileContents.Split(Environment.NewLine);
    int lineNumber = Array.IndexOf(lines, "# public key: " + publicKey);
    //Get the line above and below the public key
    string createdAtLine = lines[lineNumber - 1];
    string publicKeyLine = lines[lineNumber];
    string privateKeyLine = lines[lineNumber + 1];

    // Put the lines back together
    string rawKey = createdAtLine + Environment.NewLine + publicKeyLine + Environment.NewLine + privateKeyLine;

    // Parse the key
    var key = new AgeKey(rawKey);

    // Remove the key from the file including the new line characters
    fileContents = fileContents.Replace(rawKey, "", StringComparison.Ordinal);
    if (fileContents.EndsWith(Environment.NewLine, StringComparison.Ordinal))
      fileContents = fileContents[..^Environment.NewLine.Length];
    await File.WriteAllTextAsync(keyPath, fileContents, token).ConfigureAwait(false);

    return key;
  }

  /// <inheritdoc/>
  public async Task<AgeKey> GetKeyAsync(string publicKey, string? keyPath = null, CancellationToken token = default)
  {
    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(keyPath))
      keyPath = _sopsAgeKeyFile;

    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(keyPath, token).ConfigureAwait(false);

    // Find the line number with the public key
    string[] lines = fileContents.Split(Environment.NewLine);
    int lineNumber = Array.IndexOf(lines, "# public key: " + publicKey);
    //Get the line above and below the public key
    string createdAtLine = lines[lineNumber - 1];
    string publicKeyLine = lines[lineNumber];
    string privateKeyLine = lines[lineNumber + 1];

    // Put the lines back together
    string rawKey = createdAtLine + Environment.NewLine + publicKeyLine + Environment.NewLine + privateKeyLine;

    // Parse the key
    return new AgeKey(rawKey);
  }

  /// <inheritdoc/>
  public async Task<AgeKey> ImportKeyAsync(AgeKey inKey, string? outKeyPath = null, CancellationToken token = default)
  {
    ArgumentNullException.ThrowIfNull(inKey);

    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(outKeyPath))
      outKeyPath = _sopsAgeKeyFile;

    // Create the directory if it does not exist.
    string? directory = Path.GetDirectoryName(outKeyPath);
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      _ = Directory.CreateDirectory(directory);

    // Create the file if it does not exist.
    if (!File.Exists(outKeyPath))
    {
      using var fs = File.Create(outKeyPath);
    }

    // Append the key to the file if it does not exist.
    string fileContents = await File.ReadAllTextAsync(outKeyPath, token).ConfigureAwait(false);
    if (!fileContents.Contains(inKey.ToString(), StringComparison.Ordinal))
      await File.AppendAllTextAsync(outKeyPath, inKey.ToString(), token).ConfigureAwait(false);

    return inKey;
  }

  /// <inheritdoc/>
  public async Task<AgeKey> ImportKeyAsync(string inKeyPath, string? inKeyPublicKey = null, string? outKeyPath = null, CancellationToken token = default)
  {
    // Read the key from the in key path.
    string inKeyfileContents = await File.ReadAllTextAsync(inKeyPath, token).ConfigureAwait(false);

    // Find the line number with the public key
    string[] lines = inKeyfileContents.Split(Environment.NewLine);
    if (string.IsNullOrWhiteSpace(inKeyPublicKey) && lines.Length > 3)
      throw new InvalidOperationException("The public key must be provided if the key file contains more than one key.");
    else if (string.IsNullOrWhiteSpace(inKeyPublicKey))
      inKeyPublicKey = lines[1].Replace("# public key: ", "", StringComparison.Ordinal);

    int lineNumber = Array.IndexOf(lines, "# public key: " + inKeyPublicKey);
    //Get the line above and below the public key
    string createdAtLine = lines[lineNumber - 1];
    string publicKeyLine = lines[lineNumber];
    string privateKeyLine = lines[lineNumber + 1];

    // Put the lines back together
    string rawInKey = createdAtLine + Environment.NewLine + publicKeyLine + Environment.NewLine + privateKeyLine;

    // Parse the key
    var inKey = new AgeKey(rawInKey);

    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(outKeyPath))
      outKeyPath = _sopsAgeKeyFile;

    // Read the key from the out key path.
    string outKeyfileContents = await File.ReadAllTextAsync(outKeyPath, token).ConfigureAwait(false);

    // Append the key to the file if it does not exist.
    if (!outKeyfileContents.Contains(inKey.ToString(), StringComparison.Ordinal))
      await File.AppendAllTextAsync(outKeyPath, inKey.ToString() + Environment.NewLine, token).ConfigureAwait(false);

    return inKey;

  }

  /// <inheritdoc/>
  public async Task<bool> KeyExistsAsync(string publicKey, string? keyPath = null, CancellationToken token = default)
  {
    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(keyPath))
      keyPath = _sopsAgeKeyFile;

    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(keyPath, token).ConfigureAwait(false);

    // Check if the public key exists in the file.
    return fileContents.Contains("# public key: " + publicKey, StringComparison.Ordinal);
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<AgeKey>> ListKeysAsync(string? keyPath = null, CancellationToken token = default)
  {
    // Set the default key path if none is provided.
    if (string.IsNullOrWhiteSpace(keyPath))
      keyPath = _sopsAgeKeyFile;

    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(keyPath, token).ConfigureAwait(false);

    // Find the line number with the public key
    string[] lines = fileContents.Split(Environment.NewLine);
    List<AgeKey> keys = [];
    for (int i = 0; i < lines.Length; i++)
    {
      if (lines[i].StartsWith("# created: ", StringComparison.Ordinal))
      {
        string createdAtLine = lines[i];
        string publicKeyLine = lines[i + 1];
        string privateKeyLine = lines[i + 2];

        // Put the lines back together
        string rawKey = createdAtLine + Environment.NewLine + publicKeyLine + Environment.NewLine + privateKeyLine;

        // Parse the key
        keys.Add(new AgeKey(rawKey));
      }
    }

    return keys;
  }

  /// <summary>
  /// Get the path to the SOPS_AGE_KEY_FILE or the default path for the current OS.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  static string GetSOPSAgeKeyFilePath()
  {
    string? sopsAgeKeyFileEnvironmentVariable = Environment.GetEnvironmentVariable("SOPS_AGE_KEY_FILE");
    string sopsAgeKeyFile = "";
    if (!string.IsNullOrWhiteSpace(sopsAgeKeyFileEnvironmentVariable))
    {
      if (!File.Exists(sopsAgeKeyFileEnvironmentVariable))
        throw new ArgumentException($"The SOPS_AGE_KEY_FILE environment variable points to a file that does not exist: {sopsAgeKeyFileEnvironmentVariable}");
      sopsAgeKeyFile = sopsAgeKeyFileEnvironmentVariable;
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      sopsAgeKeyFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/sops/age/keys.txt";

    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      string xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config";
      sopsAgeKeyFile = $"{xdgConfigHome}/sops/age/keys.txt";

    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      sopsAgeKeyFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/sops/age/keys.txt";
    }
    return sopsAgeKeyFile;
  }
}
