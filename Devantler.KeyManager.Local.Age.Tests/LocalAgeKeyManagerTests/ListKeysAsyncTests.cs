namespace Devantler.KeyManager.Local.Age.Tests.LocalAgeKeyManagerTests;

/// <summary>
/// Tests for <see cref="LocalAgeKeyManager.ListKeysAsync(string?, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeKeyManager")]
public class ListKeysAsyncTests
{
  readonly LocalAgeKeyManager keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.ListKeysAsync(string?, CancellationToken)"/> returns keys from the SOPS key file when no key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ListKeysAsync_GivenNoKeyPath_ReturnsKeysFromSOPSKeyFile()
  {
    // Arrange
    var key1 = await keyManager.CreateKeyAsync();
    var key2 = await keyManager.CreateKeyAsync();

    // Act
    var keys = await keyManager.ListKeysAsync();

    // Assert
    Assert.Contains(keys, k => k.PublicKey == key1.PublicKey && k.PrivateKey == key1.PrivateKey && k.CreatedAt == key1.CreatedAt && k.ToString() == key1.ToString());
    Assert.Contains(keys, k => k.PublicKey == key2.PublicKey && k.PrivateKey == key2.PrivateKey && k.CreatedAt == key2.CreatedAt && k.ToString() == key2.ToString());

    // Cleanup
    _ = await keyManager.DeleteKeyAsync(key1);
    _ = await keyManager.DeleteKeyAsync(key2);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.ListKeysAsync(string?, CancellationToken)"/> returns keys from the specified key file when a key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ListKeysAsync_GivenKeyPath_ReturnsKeysFromSpecifiedKeyFile()
  {
    // Arrange
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    var key1 = await keyManager.CreateKeyAsync(outKeyPath);
    var key2 = await keyManager.CreateKeyAsync(outKeyPath);

    // Act
    var keys = await keyManager.ListKeysAsync(outKeyPath);

    // Assert
    Assert.Contains(keys, k => k.PublicKey == key1.PublicKey && k.PrivateKey == key1.PrivateKey && k.CreatedAt == key1.CreatedAt && k.ToString() == key1.ToString());
    Assert.Contains(keys, k => k.PublicKey == key2.PublicKey && k.PrivateKey == key2.PrivateKey && k.CreatedAt == key2.CreatedAt && k.ToString() == key2.ToString());

    // Cleanup
    _ = await keyManager.DeleteKeyAsync(key1, outKeyPath);
    _ = await keyManager.DeleteKeyAsync(key2, outKeyPath);
    File.Delete(outKeyPath);
  }

}
