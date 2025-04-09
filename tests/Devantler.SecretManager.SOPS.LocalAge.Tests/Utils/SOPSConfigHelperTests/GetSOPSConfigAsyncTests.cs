using Devantler.SecretManager.SOPS.LocalAge.Models;
using Devantler.SecretManager.SOPS.LocalAge.Utils;

namespace Devantler.SecretManager.SOPS.LocalAge.Tests.Utils.SOPSConfigHelperTests;

/// <summary>
/// Tests for <see cref="SOPSConfigHelper.GetSOPSConfigAsync(string, CancellationToken)"/>.
/// </summary>
public class GetSOPSConfigAsyncTests
{
  readonly SOPSConfigHelper _sopsConfigHelper = new();

  /// <summary>
  /// Tests that <see cref="SOPSConfigHelper.GetSOPSConfigAsync(string, CancellationToken)"/> returns the SOPS config from the file.
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
          PathRegex = @"^.+\.enc\.ya?ml$",
          EncryptedRegex = "^(data|stringData)$",
          Age = $"public-key,{Environment.NewLine}public-key"
        }
      ]
    };
    await _sopsConfigHelper.CreateSOPSConfigAsync(configPath, sopsConfig, true);

    // Act
    var result = await _sopsConfigHelper.GetSOPSConfigAsync(configPath);

    // Assert
    _ = await Verify(result);
  }
}
