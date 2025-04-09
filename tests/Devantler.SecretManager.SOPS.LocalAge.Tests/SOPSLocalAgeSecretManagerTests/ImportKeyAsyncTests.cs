using Devantler.Keys.Age;
using Devantler.SecretManager.SOPS.LocalAge.Utils;

namespace Devantler.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.ImportKeyAsync(AgeKey, CancellationToken)"/>.
/// </summary>
public class ImportKeyAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.ImportKeyAsync(AgeKey, CancellationToken)"/> imports a key into the SOPS key file.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ImportKeyAsync_GivenKey_ImportsKeyIntoSOPSKeyFile()
  {
    // Arrange
    var inKey = await AgeKeygenHelper.CreateAgeKeyAsync();

    // Act
    var outKey = await _secretManager.ImportKeyAsync(inKey);
    var outKeyFromFile = await _secretManager.GetKeyAsync(outKey.PublicKey);

    // Assert
    Assert.Equal(inKey.ToString(), outKeyFromFile.ToString());

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(inKey);
  }
}
