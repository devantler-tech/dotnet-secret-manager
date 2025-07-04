using DevantlerTech.Commons.Utils;
using DevantlerTech.Keys.Age;
using DevantlerTech.SecretManager.Core;
using DevantlerTech.SecretManager.SOPS.LocalAge.Utils;

namespace DevantlerTech.SecretManager.SOPS.LocalAge;

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

    await FileHelper.RetryFileAccessAsync(async () =>
    {
      using var fileStream = new FileStream(_sopsAgeKeyFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
      // Read the file contents.
      using var reader = new StreamReader(fileStream);
      string fileContents = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      // Append the key to the file if it does not exist.
      if (!fileContents.Contains(ageKey.ToString(), StringComparison.Ordinal))
      {
        _ = fileStream.Seek(0, SeekOrigin.End);
        using var writer = new StreamWriter(fileStream);
        await writer.WriteLineAsync(ageKey.ToString()).ConfigureAwait(false);
      }
    }, cancellationToken: cancellationToken).ConfigureAwait(false);

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
    int retryCount = 0;
    int exitCode;
    string message;
    do
    {
      (exitCode, message) = await SOPSCLI.SOPS.RunAsync(args, cancellationToken: cancellationToken).ConfigureAwait(false);

      if (exitCode == 1 && message.Contains("process cannot access the file", StringComparison.OrdinalIgnoreCase))
      {
        retryCount++;
        await Task.Delay(100, cancellationToken).ConfigureAwait(false);
      }
    } while (exitCode == 1 && message.Contains("process cannot access the file", StringComparison.OrdinalIgnoreCase) && retryCount < 3);
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

    await FileHelper.RetryFileAccessAsync(async () =>
    {
      using var fileStream = new FileStream(_sopsAgeKeyFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
      // Read the file contents.
      using var reader = new StreamReader(fileStream);
      string fileContents = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      if (fileContents.Contains(key.ToString(), StringComparison.Ordinal))
      {
        fileContents = fileContents.Replace(key.ToString() + Environment.NewLine, "", StringComparison.Ordinal);
        fileContents = fileContents.Replace(key.ToString(), "", StringComparison.Ordinal);
        fileContents = fileContents.TrimEnd() + Environment.NewLine;

        // Write the updated contents back to the file.
        fileStream.SetLength(0);
        using var writer = new StreamWriter(fileStream);
        await writer.WriteAsync(fileContents).ConfigureAwait(false);
      }
    }, cancellationToken: cancellationToken).ConfigureAwait(false);

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
    AgeKey? key = null;

    await FileHelper.RetryFileAccessAsync(async () =>
    {
      using var fileStream = new FileStream(_sopsAgeKeyFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
      // Read the file contents.
      using var reader = new StreamReader(fileStream);
      string fileContents = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      // Find the line number with the public key
      string[] lines = fileContents.Split(Environment.NewLine);
      int lineNumber = Array.IndexOf(lines, "# public key: " + publicKey);
      if (lineNumber == -1)
        throw new SecretManagerException("The key does not exist in the key file.");

      // Get the line above and below the public key
      string createdAtLine = lines[lineNumber - 1];
      string publicKeyLine = lines[lineNumber];
      string privateKeyLine = lines[lineNumber + 1];

      // Put the lines back together
      string rawKey = createdAtLine + Environment.NewLine + publicKeyLine + Environment.NewLine + privateKeyLine;

      // Parse the key
      key = new AgeKey(rawKey);

      // Remove the key from the file including the new line characters
      fileContents = fileContents.Replace(rawKey, "", StringComparison.Ordinal);
      fileContents = fileContents.TrimEnd() + Environment.NewLine;

      // Write the updated contents back to the file
      fileStream.SetLength(0);
      using var writer = new StreamWriter(fileStream);
      await writer.WriteAsync(fileContents).ConfigureAwait(false);
    }, cancellationToken: cancellationToken).ConfigureAwait(false);

    return key ?? throw new SecretManagerException("Failed to delete the key due to file access issues.");
  }

  /// <inheritdoc/>
  public async Task EditAsync(string filePath, CancellationToken cancellationToken = default)
  {
    List<string> args = ["edit", filePath];
    _ = await SOPSCLI.SOPS.RunAsync([.. args], cancellationToken: cancellationToken).ConfigureAwait(false);
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
    AgeKey? key = null;

    await FileHelper.RetryFileAccessAsync(async () =>
    {
      using var fileStream = new FileStream(_sopsAgeKeyFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
      using var reader = new StreamReader(fileStream);

      // Get the contents of the file.
      string fileContents = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      if (!fileContents.Contains("# public key: " + publicKey, StringComparison.Ordinal))
        throw new SecretManagerException("The key does not exist in the key file.");

      // Find the line number with the public key
      string[] lines = fileContents.Split(Environment.NewLine);
      int lineNumber = Array.IndexOf(lines, "# public key: " + publicKey);

      // Get the line above and below the public key
      string createdAtLine = lines[lineNumber - 1];
      string publicKeyLine = lines[lineNumber];
      string privateKeyLine = lines[lineNumber + 1];

      // Put the lines back together
      string rawKey = createdAtLine + Environment.NewLine + publicKeyLine + Environment.NewLine + privateKeyLine;

      // Parse the key
      key = new AgeKey(rawKey);
    }, cancellationToken: cancellationToken).ConfigureAwait(false);

    return key ?? throw new SecretManagerException("Failed to retrieve the key due to file access issues.");
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

    await FileHelper.RetryFileAccessAsync(async () =>
    {
      using var fileStream = new FileStream(_sopsAgeKeyFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
      // Read the file contents.
      using var reader = new StreamReader(fileStream);
      string fileContents = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      // Append the key to the file if it does not exist.
      if (!fileContents.Contains(key.ToString(), StringComparison.Ordinal))
      {
        _ = fileStream.Seek(0, SeekOrigin.End);
        using var writer = new StreamWriter(fileStream);
        await writer.WriteLineAsync(key.ToString()).ConfigureAwait(false);
      }
    }, cancellationToken: cancellationToken).ConfigureAwait(false);

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
    bool keyExists = false;

    await FileHelper.RetryFileAccessAsync(async () =>
    {
      using var fileStream = new FileStream(_sopsAgeKeyFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
      using var reader = new StreamReader(fileStream);

      // Get the contents of the file.
      string fileContents = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      // Check if the public key exists in the file.
      keyExists = fileContents.Contains("# public key: " + publicKey, StringComparison.Ordinal);
    }, cancellationToken: cancellationToken).ConfigureAwait(false);

    return keyExists;
  }

  /// <summary>
  /// List all Age keys in the SOPS Age key file.
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public async Task<IEnumerable<AgeKey>> ListKeysAsync(CancellationToken cancellationToken = default)
  {
    var keys = new List<AgeKey>();

    if (!File.Exists(_sopsAgeKeyFilePath))
      return [];

    await FileHelper.RetryFileAccessAsync(async () =>
    {
      using var fileStream = new FileStream(_sopsAgeKeyFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
      using var reader = new StreamReader(fileStream);

      string fileContents = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      // Find the line number with the public key
      string[] lines = fileContents.Split(Environment.NewLine);
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
    }, cancellationToken: cancellationToken).ConfigureAwait(false);

    return keys;
  }
}
