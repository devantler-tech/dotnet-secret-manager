using Devantler.Keys.Age;

namespace Devantler.KeyManager.Local.Age.Tests.LocalAgeKeyManagerTests;

/// <summary>
/// Tests for <see cref="LocalAgeKeyManager.DeleteKeyAsync(AgeKey, string?, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeKeyManager")]
public class DeleteKeyAsyncTests
{
  readonly LocalAgeKeyManager _keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.DeleteKeyAsync(AgeKey, string?, CancellationToken)"/> deletes a key from the SOPS key file when no key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task DeleteKeyAsync_GivenKeyAndNoKeyPath_DeletesKeyFromSOPSKeyFile()
  {
    // Arrange
    var key = await _keyManager.CreateKeyAsync();

    // Act
    _ = await _keyManager.DeleteKeyAsync(key);
    bool keyExists = await _keyManager.KeyExistsAsync(key.PublicKey);

    // Assert
    Assert.False(keyExists);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.DeleteKeyAsync(AgeKey, string?, CancellationToken)"/> deletes a key from the specified key file when a key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task DeleteKeyAsync_GivenKeyAndKeyPath_DeletesKeyFromSpecifiedKeyFile()
  {
    // Arrange
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    var key = await _keyManager.CreateKeyAsync(outKeyPath);

    // Act
    _ = await _keyManager.DeleteKeyAsync(key, outKeyPath);
    bool keyExists = await _keyManager.KeyExistsAsync(key.PublicKey, outKeyPath);

    // Assert
    Assert.False(keyExists);

    // Cleanup
    File.Delete(outKeyPath);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.DeleteKeyAsync(AgeKey, string?, CancellationToken)"/> deletes a key from the SOPS key file when no key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task DeleteKeyAsync_GivenPublicKeyAndNoKeyPath_DeletesKeyFromSOPSKeyFile()
  {
    // Arrange
    var key = await _keyManager.CreateKeyAsync();

    // Act
    _ = await _keyManager.DeleteKeyAsync(key.PublicKey);
    bool keyExists = await _keyManager.KeyExistsAsync(key.PublicKey);

    // Assert
    Assert.False(keyExists);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.DeleteKeyAsync(AgeKey, string?, CancellationToken)"/> deletes a key from the specified key file when a key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task DeleteKeyAsync_GivenPublicKeyAndKeyPath_DeletesKeyFromSpecifiedKeyFile()
  {
    // Arrange
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    var key = await _keyManager.CreateKeyAsync(outKeyPath);

    // Act
    _ = await _keyManager.DeleteKeyAsync(key.PublicKey, outKeyPath);
    bool keyExists = await _keyManager.KeyExistsAsync(key.PublicKey, outKeyPath);

    // Assert
    Assert.False(keyExists);

    // Cleanup
    File.Delete(outKeyPath);
  }
}
