using Devantler.Keys.Age;
using Devantler.SecretManager.Core;
using Devantler.SecretManager.SOPS.LocalAge.Utils;

namespace Devantler.SecretManager.SOPS.LocalAge;

/// <summary>
/// A local secret manager for SOPS with Age keys.
/// </summary>
public class SOPSLocalAgeSecretManager : ISecretManager<AgeKey>
{
  readonly string _sopsAgeKeyFilePath = SOPSAgeKeyFileHelper.GetSOPSAgeKeyFilePath();

  /// <summary>
  /// Create a new instance of the <see cref="SOPSLocalAgeSecretManager"/> class.
  /// </summary>
  public SOPSLocalAgeSecretManager() => _sopsAgeKeyFilePath = SOPSAgeKeyFileHelper.GetSOPSAgeKeyFilePath();

  /// <summary>
  /// Create a new instance of the <see cref="SOPSLocalAgeSecretManager"/> class with a custom path to the SOPS Age key file.
  /// </summary>
  /// <param name="sopsAgeKeyFilePath"></param>
  public SOPSLocalAgeSecretManager(string sopsAgeKeyFilePath) => _sopsAgeKeyFilePath = sopsAgeKeyFilePath;

  /// <summary>
  /// Create a new Age key.
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<AgeKey> CreateKeyAsync(CancellationToken cancellationToken = default)
  {
    // Create a new Age key.
    var ageKey = await AgeKeygenHelper.CreateAgeKeyAsync(cancellationToken).ConfigureAwait(false);

    // Create the directory if it does not exist.
    string? directory = Path.GetDirectoryName(_sopsAgeKeyFilePath);
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      _ = Directory.CreateDirectory(directory);

    // Create the file if it does not exist.
    if (!File.Exists(_sopsAgeKeyFilePath))
    {
      using var fs = File.Create(_sopsAgeKeyFilePath);
    }

    // Append the key to the file if it does not exist.
    string fileContents = await File.ReadAllTextAsync(_sopsAgeKeyFilePath, cancellationToken).ConfigureAwait(false);
    if (!fileContents.Contains(ageKey.ToString(), StringComparison.Ordinal))
      await File.AppendAllTextAsync(_sopsAgeKeyFilePath, ageKey.ToString() + Environment.NewLine, cancellationToken).ConfigureAwait(false);

    return ageKey;
  }

  /// <summary>
  /// Decrypt a file with an Age private key.
  /// </summary>
  /// <param name="filePath"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="SecretManagerException"></exception>
  public async Task<string> DecryptAsync(string filePath, CancellationToken cancellationToken = default)
  {
    string[] args = ["decrypt", filePath];
    var (exitCode, message) = await SOPSCLI.SOPS.RunAsync(args, cancellationToken: cancellationToken).ConfigureAwait(false);
    return exitCode != 0 ? throw new SecretManagerException(message) : message;
  }

  /// <summary>
  /// Delete an Age key.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<AgeKey> DeleteKeyAsync(AgeKey key, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(key, nameof(key));
    // Delete the key from the file.
    string fileContents = await File.ReadAllTextAsync(_sopsAgeKeyFilePath, cancellationToken).ConfigureAwait(false);
    if (fileContents.Contains(key.ToString(), StringComparison.Ordinal))
    {
      fileContents = fileContents.Replace(key.ToString() + Environment.NewLine, "", StringComparison.Ordinal);
      fileContents = fileContents.Replace(key.ToString(), "", StringComparison.Ordinal);
      fileContents = fileContents.TrimEnd() + Environment.NewLine;
      await File.WriteAllTextAsync(_sopsAgeKeyFilePath, fileContents, cancellationToken).ConfigureAwait(false);
    }

    return key;
  }

  /// <summary>
  /// Delete an Age key by its public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<AgeKey> DeleteKeyAsync(string publicKey, CancellationToken cancellationToken = default)
  {
    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(_sopsAgeKeyFilePath, cancellationToken).ConfigureAwait(false);

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
    await File.WriteAllTextAsync(_sopsAgeKeyFilePath, fileContents, cancellationToken).ConfigureAwait(false);

    return key;
  }

  /// <inheritdoc/>
  public async Task EditAsync(string filePath, CancellationToken cancellationToken = default)
  {
    List<string> args = ["edit", filePath];
    await SOPSCLI.SOPS.RunAsync([.. args], cancellationToken: cancellationToken);
  }

  /// <summary>
  /// Encrypt a file with an Age public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="filePath"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<string> EncryptAsync(string filePath, string? publicKey, CancellationToken cancellationToken = default)
  {
    List<string> args = ["encrypt"];
    if (!string.IsNullOrEmpty(publicKey))
    {
      args.Add("--age");
      args.Add(publicKey);
    }
    args.Add(filePath);

    var (exitCode, message) = await SOPSCLI.SOPS.RunAsync([.. args], cancellationToken: cancellationToken).ConfigureAwait(false);
    return exitCode != 0 ? throw new SecretManagerException(message) : message;
  }

  /// <summary>
  /// Get an Age key by its public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="SecretManagerException"></exception>
  public async Task<AgeKey> GetKeyAsync(string publicKey, CancellationToken cancellationToken = default)
  {
    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(_sopsAgeKeyFilePath, cancellationToken).ConfigureAwait(false);

    if (!fileContents.Contains("# public key: " + publicKey, StringComparison.Ordinal))
      throw new SecretManagerException("the key does not exist in the key file.");

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
  /// Import an Age key.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<AgeKey> ImportKeyAsync(AgeKey key, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(key, nameof(key));
    // Create the directory if it does not exist.
    string? directory = Path.GetDirectoryName(_sopsAgeKeyFilePath);
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      _ = Directory.CreateDirectory(directory);

    // Create the file if it does not exist.
    if (!File.Exists(_sopsAgeKeyFilePath))
    {
      using var fs = File.Create(_sopsAgeKeyFilePath);
    }

    // Append the key to the file if it does not exist.
    string fileContents = await File.ReadAllTextAsync(_sopsAgeKeyFilePath, cancellationToken).ConfigureAwait(false);
    if (!fileContents.Contains(key.ToString(), StringComparison.Ordinal))
      await File.AppendAllTextAsync(_sopsAgeKeyFilePath, key.ToString(), cancellationToken).ConfigureAwait(false);

    return key;
  }

  /// <summary>
  /// Check if an Age key exists in the SOPS Age key file.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public async Task<bool> KeyExistsAsync(string publicKey, CancellationToken cancellationToken = default)
  {
    // Get the contents of the file.
    string fileContents = await File.ReadAllTextAsync(_sopsAgeKeyFilePath, cancellationToken).ConfigureAwait(false);

    // Check if the public key exists in the file.
    return fileContents.Contains("# public key: " + publicKey, StringComparison.Ordinal);
  }

  /// <summary>
  /// List all Age keys in the SOPS Age key file.
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public async Task<IEnumerable<AgeKey>> ListKeysAsync(CancellationToken cancellationToken = default)
  {
    if (!File.Exists(_sopsAgeKeyFilePath))
      return [];

    string fileContents = await File.ReadAllTextAsync(_sopsAgeKeyFilePath, cancellationToken).ConfigureAwait(false);

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
}
