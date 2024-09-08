namespace Devantler.KeyManager.Local.Age.Tests.LocalAgeKeyManagerTests;

/// <summary>
/// Tests for <see cref="LocalAgeKeyManager.GetKeyAsync(string, string?, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeKeyManager")]
public class GetKeyAsyncTests
{
  readonly LocalAgeKeyManager _keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.GetKeyAsync(string, string?, CancellationToken)"/> gets a key from the SOPS key file when no key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetKeyAsync_GivenPublicKeyAndNoKeyPath_ReturnsKeyFromSOPSKeyFile()
  {
    // Arrange
    var key = await _keyManager.CreateKeyAsync();

    // Act
    var keyFromFile = await _keyManager.GetKeyAsync(key.PublicKey);

    // Assert
    Assert.Equal(key.ToString(), keyFromFile.ToString());

    // Cleanup
    _ = await _keyManager.DeleteKeyAsync(key);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.GetKeyAsync(string, string?, CancellationToken)"/> gets a key from the specified key file when a key path is provided.
  /// </summary>
  [Fact]
  public async Task GetKeyAsync_GivenPublicKeyAndKeyPath_ReturnsKeyFromSpecifiedKeyFile()
  {
    // Arrange
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    var key = await _keyManager.CreateKeyAsync(outKeyPath);

    // Act
    var keyFromFile = await _keyManager.GetKeyAsync(key.PublicKey, outKeyPath);

    // Assert
    Assert.Equal(key.ToString(), keyFromFile.ToString());

    // Cleanup
    File.Delete(outKeyPath);
  }
}
