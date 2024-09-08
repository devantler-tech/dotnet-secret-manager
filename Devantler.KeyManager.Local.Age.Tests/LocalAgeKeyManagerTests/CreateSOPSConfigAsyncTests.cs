using Devantler.KeyManager.Core.Models;

namespace Devantler.KeyManager.Local.Age.Tests.LocalAgeKeyManagerTests;
/// <summary>
/// Tests for <see cref="LocalAgeKeyManager.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeKeyManager")]
public class GenerateSOPSConfigAsyncTests
{
  readonly LocalAgeKeyManager keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/> creates a new SOPS config file when the file does not exist.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task CreateSOPSConfigAsync_GivenNewConfigPathAndValidSOPSConfig_CreatesNewConfigFile()
  {
    // Arrange
    string configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    SOPSConfig sopsConfig = new()
    {
      CreationRules =
      [
        new SOPSConfigCreationRule
        {
          PathRegex = ".sops.yaml",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };

    // Act
    await keyManager.CreateSOPSConfigAsync(configPath, sopsConfig);
    string configFromFile = await File.ReadAllTextAsync(configPath);

    // Assert
    _ = await Verify(configFromFile);

    // Cleanup
    File.Delete(configPath);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/> overwrites an existing SOPS config file when the file exists and overwrite is true.
  /// </summary>
  [Fact]
  public async Task CreateSOPSConfigAsync_GivenExistingConfigPathAndValidSOPSConfigAndOverwriteTrue_OverwritesExistingConfigFile()
  {
    // Arrange
    string configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    SOPSConfig sopsConfig = new()
    {
      CreationRules =
      [
        new SOPSConfigCreationRule
        {
          PathRegex = ".sops.yaml",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };

    // Act
    await keyManager.CreateSOPSConfigAsync(configPath, sopsConfig);
    string configFromFile = await File.ReadAllTextAsync(configPath);
    sopsConfig.CreationRules.Add(new SOPSConfigCreationRule
    {
      PathRegex = ".sops.yaml",
      EncryptedRegex = "^(data|stringData)$",
      Age = $"public-key,{Environment.NewLine}public-key"
    });
    await keyManager.CreateSOPSConfigAsync(configPath, sopsConfig, true);
    string configFromFileAfterOverwrite = await File.ReadAllTextAsync(configPath);

    // Assert
    Assert.NotEqual(configFromFile, configFromFileAfterOverwrite);
    _ = await Verify($"{configFromFile}{Environment.NewLine}{configFromFileAfterOverwrite}");

    // Cleanup
    File.Delete(configPath);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/> creates directories when the directory does not exist.
  /// </summary>
  [Fact]
  public async Task CreateSOPSConfigAsync_GivenNewConfigPathWithNonExistentDirectoryAndValidSOPSConfig_CreatesDirectoryAndConfigFile()
  {
    // Arrange
    string tempPath = Path.GetTempPath();
    string directoryPath = Path.Combine(tempPath, "first-dir", "second-dir");
    string configPath = Path.Combine(directoryPath, Path.GetRandomFileName());
    SOPSConfig sopsConfig = new()
    {
      CreationRules =
      [
        new SOPSConfigCreationRule
        {
          PathRegex = ".sops.yaml",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };

    // Act
    await keyManager.CreateSOPSConfigAsync(configPath, sopsConfig);
    string configFromFile = await File.ReadAllTextAsync(configPath);

    // Assert
    Assert.True(Directory.Exists(directoryPath));
    _ = await Verify(configFromFile);

    // Cleanup
    Directory.Delete(directoryPath, true);
  }
}
