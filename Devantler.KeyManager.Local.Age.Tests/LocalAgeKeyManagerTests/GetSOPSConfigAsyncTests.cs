using Devantler.KeyManager.Core.Models;

namespace Devantler.KeyManager.Local.Age.Tests.LocalAgeKeyManagerTests;

/// <summary>
/// Tests for <see cref="LocalAgeKeyManager.GetSOPSConfigAsync(string, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeKeyManager")]
public class GetSOPSConfigAsyncTests
{
  readonly LocalAgeKeyManager keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.GetSOPSConfigAsync(string, CancellationToken)"/> returns the SOPS config from the file.
  /// </summary>
  [Fact]
  public async Task GetSOPSConfigAsync_GivenValidConfigPath_ReturnsSOPSConfig()
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
    await keyManager.CreateSOPSConfigAsync(configPath, sopsConfig, true);

    // Act
    var result = await keyManager.GetSOPSConfigAsync(configPath);

    // Assert
    _ = await Verify(result);
  }
}
