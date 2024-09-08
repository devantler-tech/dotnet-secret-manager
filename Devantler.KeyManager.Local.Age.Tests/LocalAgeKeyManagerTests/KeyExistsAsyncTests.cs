namespace Devantler.KeyManager.Local.Age.Tests.LocalAgeKeyManagerTests;

/// <summary>
/// Tests for <see cref="LocalAgeKeyManager.KeyExistsAsync(string, string, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeKeyManager")]
public class KeyExistsAsyncTests
{
  readonly LocalAgeKeyManager _keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.KeyExistsAsync(string, string, CancellationToken)"/> returns true if a key exists in the SOPS key file when no key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task KeyExistsAsync_GivenPublicKeyAndNoKeyPath_ReturnsTrueIfKeyExistsInSOPSKeyFile()
  {
    // Arrange
    var key = await _keyManager.CreateKeyAsync();

    // Act
    bool keyExists = await _keyManager.KeyExistsAsync(key.PublicKey);

    // Assert
    Assert.True(keyExists);

    // Cleanup
    _ = await _keyManager.DeleteKeyAsync(key);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.KeyExistsAsync(string, string, CancellationToken)"/> returns true if a key exists in the specified key file when a key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task KeyExistsAsync_GivenPublicKeyAndKeyPath_ReturnsTrueIfKeyExistsInSpecifiedKeyFile()
  {
    // Arrange
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    var key = await _keyManager.CreateKeyAsync(outKeyPath);

    // Act
    bool keyExists = await _keyManager.KeyExistsAsync(key.PublicKey, outKeyPath);

    // Assert
    Assert.True(keyExists);

    // Cleanup
    File.Delete(outKeyPath);
  }
}
