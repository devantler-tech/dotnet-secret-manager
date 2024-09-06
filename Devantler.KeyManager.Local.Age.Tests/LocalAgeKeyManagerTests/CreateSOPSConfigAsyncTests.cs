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
  /// Tests that <see cref="LocalAgeKeyManager.CreateKeyAsync(CancellationToken)"/> creates a key in the SOPS key file when no out key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GenerateSOPSConfigAsync_GivenNewConfigPathAndValidSOPSConfig_CreatesNewConfigFile()
  {
    // Arrange
    string configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    SOPSConfig sopsConfig = new()
    {
      CreationRules =
      [
        new SOPSConfigCreationRule{
          PathRegex = ".sops.yaml",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };

    // Act
    await keyManager.CreateSOPSConfigAsync(configPath, sopsConfig, true);
    string configFromFile = await File.ReadAllTextAsync(configPath);

    // Assert
    _ = await Verify(configFromFile);

    // Cleanup
    File.Delete(configPath);
  }
}
