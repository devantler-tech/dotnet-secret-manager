using DevantlerTech.SecretManager.SOPS.LocalAge.Models;
using DevantlerTech.SecretManager.SOPS.LocalAge.Utils;

namespace DevantlerTech.SecretManager.SOPS.LocalAge.Tests.Utils.SOPSConfigHelperTests;
/// <summary>
/// Tests for <see cref="SOPSConfigHelper.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/>.
/// </summary>
public class CreateSOPSConfigAsyncTests
{
  readonly SOPSConfigHelper _sopsConfigHelper = new();

  /// <summary>
  /// Tests that <see cref="SOPSConfigHelper.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/> creates a new SOPS config file when the file does not exist.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task CreateSOPSConfigAsync_CreatesNewConfigFile()
  {
    // Arrange
    string configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    SOPSConfig sopsConfig = new()
    {
      CreationRules =
      [
        new SOPSConfigCreationRule
        {
          PathRegex = @"^.+\.enc\.ya?ml$",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };

    // Act
    await _sopsConfigHelper.CreateSOPSConfigAsync(configPath, sopsConfig);
    string configFromFile = await File.ReadAllTextAsync(configPath);

    // Assert
    _ = await Verify(configFromFile);

    // Cleanup
    File.Delete(configPath);
  }

  /// <summary>
  /// Tests that <see cref="SOPSConfigHelper.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/> overwrites an existing SOPS config file when the file exists and overwrite is true.
  /// </summary>
  [Fact]
  public async Task CreateSOPSConfigAsync_OverwritesExistingConfigFile()
  {
    // Arrange
    string configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    SOPSConfig sopsConfig = new()
    {
      CreationRules =
      [
        new SOPSConfigCreationRule
        {
          PathRegex = @"^.+\.enc\.ya?ml$",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };

    // Act
    await _sopsConfigHelper.CreateSOPSConfigAsync(configPath, sopsConfig);
    string configFromFile = await File.ReadAllTextAsync(configPath);
    sopsConfig.CreationRules.Add(new SOPSConfigCreationRule
    {
      PathRegex = @"^.+\.enc\.ya?ml$",
      EncryptedRegex = "^(data|stringData)$",
      Age = $"public-key,{Environment.NewLine}public-key"
    });
    await _sopsConfigHelper.CreateSOPSConfigAsync(configPath, sopsConfig, true);
    string configFromFileAfterOverwrite = await File.ReadAllTextAsync(configPath);

    // Assert
    Assert.NotEqual(configFromFile, configFromFileAfterOverwrite);
    _ = await Verify($"{configFromFile}{Environment.NewLine}{configFromFileAfterOverwrite}");

    // Cleanup
    File.Delete(configPath);
  }

  /// <summary>
  /// Tests that <see cref="SOPSConfigHelper.CreateSOPSConfigAsync(string, SOPSConfig, bool, CancellationToken)"/> creates directories when the directory does not exist.
  /// </summary>
  [Fact]
  public async Task CreateSOPSConfigAsync_CreatesDirectoryAndConfigFile()
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
          PathRegex = @"^.+\.enc\.ya?ml$",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };

    // Act
    await _sopsConfigHelper.CreateSOPSConfigAsync(configPath, sopsConfig);
    string configFromFile = await File.ReadAllTextAsync(configPath);

    // Assert
    Assert.True(Directory.Exists(directoryPath));
    _ = await Verify(configFromFile);

    // Cleanup
    Directory.Delete(directoryPath, true);
  }
}
