using DevantlerTech.SecretManager.SOPS.LocalAge.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DevantlerTech.SecretManager.SOPS.LocalAge.Utils;

/// <summary>
/// Helper class to manage SOPS configuration files.
/// </summary>
public class SOPSConfigHelper
{
  IDeserializer YAMLDeserializer { get; } = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
  ISerializer YAMLSerializer { get; } = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();

  /// <summary>
  /// Get the SOPS configuration from a file.
  /// </summary>
  /// <param name="configPath"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<SOPSConfig> GetSOPSConfigAsync(string configPath, CancellationToken cancellationToken = default)
  {
    string configContents = await File.ReadAllTextAsync(configPath, cancellationToken).ConfigureAwait(false);
    var config = YAMLDeserializer.Deserialize<SOPSConfig>(configContents);
    return config;
  }

  /// <summary>
  /// Create a new SOPS configuration file.
  /// </summary>
  /// <param name="configPath"></param>
  /// <param name="config"></param>
  /// <param name="overwrite"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public async Task CreateSOPSConfigAsync(string configPath, SOPSConfig config, bool overwrite = false, CancellationToken cancellationToken = default)
  {
    if (!overwrite && File.Exists(configPath))
      throw new InvalidOperationException("The file already exists and overwrite is set to false.");

    // Create the directory if it does not exist.
    string? directory = Path.GetDirectoryName(configPath);
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      _ = Directory.CreateDirectory(directory);

    string configRaw = YAMLSerializer.Serialize(config);
    await File.WriteAllTextAsync(configPath, configRaw, cancellationToken).ConfigureAwait(false);
  }
}
