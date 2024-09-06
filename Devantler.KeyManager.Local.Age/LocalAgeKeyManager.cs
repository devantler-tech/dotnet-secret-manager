using Devantler.Keys.Age;
using Devantler.KeyManager.Core;
using Devantler.AgeCLI;
using System.Runtime.InteropServices;
using Devantler.KeyManager.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Devantler.KeyManager.Local.Age;

/// <summary>
/// A local key manager for SOPS with Age keys.
/// </summary>
public class LocalAgeKeyManager() : ILocalKeyManager<AgeKey>
{
  readonly string _sopsAgeKeyFile = GetSOPSAgeKeyFilePath();
  IDeserializer YAMLDeserializer { get; } = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
  ISerializer YAMLSerializer { get; } = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();

  /// <summary>
  /// Create a new key, and add it to the default key file.
  /// </summary>
  /// <param name="token"></param>
  /// <returns><see cref="AgeKey"/></returns>
  public async Task<AgeKey> CreateKeyAsync(CancellationToken token = default) => await CreateKeyAsync(_sopsAgeKeyFile, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<AgeKey> CreateKeyAsync(string outKeyPath, CancellationToken token = default)
  {
    // Create a new Age key.
    var key = await AgeKeygen.InMemory(token).ConfigureAwait(false);

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

  /// <summary>
  /// Delete a key from the default key file.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="token"></param>
  /// <returns><see cref="AgeKey"/></returns>
  public async Task<AgeKey> DeleteKeyAsync(AgeKey key, CancellationToken token = default) => await DeleteKeyAsync(key, _sopsAgeKeyFile, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<AgeKey> DeleteKeyAsync(AgeKey key, string keyPath, CancellationToken token = default)
  {
    ArgumentNullException.ThrowIfNull(key);

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

  /// <summary>
  /// Delete a key by public key from the default key file.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns><see cref="AgeKey"/></returns>
  public async Task<AgeKey> DeleteKeyAsync(string publicKey, CancellationToken token = default) => await DeleteKeyAsync(publicKey, _sopsAgeKeyFile, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<AgeKey> DeleteKeyAsync(string publicKey, string keyPath, CancellationToken token = default)
  {
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

  /// <summary>
  /// Get a key by public key from the default key file.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns><see cref="AgeKey"/></returns>
  public async Task<AgeKey> GetKeyAsync(string publicKey, CancellationToken token = default) => await GetKeyAsync(publicKey, _sopsAgeKeyFile, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<AgeKey> GetKeyAsync(string publicKey, string keyPath, CancellationToken token = default)
  {
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

  /// <summary>
  /// Import a key from a Key object to the default key file.
  /// </summary>
  /// <param name="inKey"></param>
  /// <param name="token"></param>
  /// <returns><see cref="AgeKey"/></returns>
  public async Task<AgeKey> ImportKeyAsync(AgeKey inKey, CancellationToken token = default) => await ImportKeyAsync(inKey, _sopsAgeKeyFile, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<AgeKey> ImportKeyAsync(AgeKey inKey, string outKeyPath, CancellationToken token = default)
  {
    ArgumentNullException.ThrowIfNull(inKey);

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

  /// <summary>
  /// Import a key from a file to the default key file.
  /// </summary>
  /// <param name="inKeyPath"></param>
  /// <param name="inKeyPublicKey"></param>
  /// <param name="token"></param>
  /// <returns><see cref="AgeKey"/></returns>
  public async Task<AgeKey> ImportKeyAsync(string inKeyPath, string? inKeyPublicKey = null, CancellationToken token = default) => await ImportKeyAsync(inKeyPath, _sopsAgeKeyFile, inKeyPublicKey, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<AgeKey> ImportKeyAsync(string inKeyPath, string outKeyPath, string? inKeyPublicKey = null, CancellationToken token = default)
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

    // Read the key from the out key path.
    string outKeyfileContents = await File.ReadAllTextAsync(outKeyPath, token).ConfigureAwait(false);

    // Append the key to the file if it does not exist.
    if (!outKeyfileContents.Contains(inKey.ToString(), StringComparison.Ordinal))
      await File.AppendAllTextAsync(outKeyPath, inKey.ToString() + Environment.NewLine, token).ConfigureAwait(false);

    return inKey;

  }

  /// <summary>
  /// Check if a key exists in the default key file.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns><see cref="bool"/></returns>
  public async Task<bool> KeyExistsAsync(string publicKey, CancellationToken token = default) => await KeyExistsAsync(publicKey, _sopsAgeKeyFile, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<bool> KeyExistsAsync(string publicKey, string keyPath, CancellationToken token = default)
  {
    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(keyPath, token).ConfigureAwait(false);

    // Check if the public key exists in the file.
    return fileContents.Contains("# public key: " + publicKey, StringComparison.Ordinal);
  }

  /// <summary>
  /// List all keys from the default key file.
  /// </summary>
  /// <param name="token"></param>
  /// <returns><see cref="IEnumerable{AgeKey}"/></returns>
  public async Task<IEnumerable<AgeKey>> ListKeysAsync(CancellationToken token = default) => await ListKeysAsync(_sopsAgeKeyFile, token).ConfigureAwait(false);

  /// <inheritdoc/>
  public async Task<IEnumerable<AgeKey>> ListKeysAsync(string keyPath, CancellationToken token = default)
  {
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

  /// <inheritdoc/>
  public async Task<SOPSConfig> GetSOPSConfigAsync(string configPath, CancellationToken token = default)
  {
    string configContents = await File.ReadAllTextAsync(configPath, token).ConfigureAwait(false);
    var config = YAMLDeserializer.Deserialize<SOPSConfig>(configContents);
    return config;
  }

  /// <inheritdoc/>
  public async Task CreateSOPSConfigAsync(string configPath, SOPSConfig config, bool overwrite = false, CancellationToken token = default)
  {
    if (overwrite && File.Exists(configPath))
      File.Delete(configPath);
    string configRaw = YAMLSerializer.Serialize(config);
    await File.WriteAllTextAsync(configPath, configRaw, token).ConfigureAwait(false);
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
