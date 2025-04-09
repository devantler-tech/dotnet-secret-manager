namespace Devantler.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.KeyExistsAsync(string, CancellationToken)"/>.
/// </summary>
public class KeyExistsAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.KeyExistsAsync(string, CancellationToken)"/> returns true if a key exists in the SOPS key file.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task KeyExistsAsync_GivenPublicKey_ReturnsTrueIfKeyExistsInSOPSKeyFile()
  {
    // Arrange
    var key = await _secretManager.CreateKeyAsync();

    // Act
    bool keyExists = await _secretManager.KeyExistsAsync(key.PublicKey);

    // Assert
    Assert.True(keyExists);

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(key);
  }
}
