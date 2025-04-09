using Devantler.Keys.Age;

namespace Devantler.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.DeleteKeyAsync(AgeKey, CancellationToken)"/>.
/// </summary>
public class DeleteKeyAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.DeleteKeyAsync(AgeKey, CancellationToken)"/> deletes a key from the SOPS key file when no key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task DeleteKeyAsyncWithAgeKey_DeletesKeyFromSOPSKeyFile()
  {
    // Arrange
    var key = await _secretManager.CreateKeyAsync();

    // Act
    _ = await _secretManager.DeleteKeyAsync(key);
    bool keyExists = await _secretManager.KeyExistsAsync(key.PublicKey);

    // Assert
    Assert.False(keyExists);
  }

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.DeleteKeyAsync(string, CancellationToken)"/> deletes a key from the SOPS key file when no key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task DeleteKeyAsyncWithPublicKeyString_DeletesKeyFromSOPSKeyFile()
  {
    // Arrange
    var key = await _secretManager.CreateKeyAsync();

    // Act
    _ = await _secretManager.DeleteKeyAsync(key.PublicKey);
    bool keyExists = await _secretManager.KeyExistsAsync(key.PublicKey);

    // Assert
    Assert.False(keyExists);
  }
}
