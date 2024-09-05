namespace Devantler.KeyManager.Local.Age.Tests.LocalAgeKeyManagerTests;

/// <summary>
/// Tests for <see cref="LocalAgeKeyManager.CreateKeyAsync(string, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeKeyManager")]
public class CreateKeyAsyncTests
{
  readonly LocalAgeKeyManager keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.CreateKeyAsync(CancellationToken)"/> creates a key in the SOPS key file when no out key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task CreateKeyAsync_GivenNoOutKeyPath_CreatesKeyInSOPSKeyFile()
  {
    // Act
    var key = await keyManager.CreateKeyAsync();
    var keyFromFile = await keyManager.GetKeyAsync(key.PublicKey);

    // Assert
    Assert.Equal(key.ToString(), keyFromFile.ToString());

    // Cleanup
    _ = await keyManager.DeleteKeyAsync(key);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeKeyManager.CreateKeyAsync(string, CancellationToken)"/> creates a key in the specified key file when an out key path is provided.
  /// </summary>
  [Fact]
  public async Task CreateKeyAsync_GivenOutKeyPath_CreatesKeyInSpecifiedKeyFile()
  {
    // Arrange
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    // Act
    var key = await keyManager.CreateKeyAsync(outKeyPath);
    var keyFromFile = await keyManager.GetKeyAsync(key.PublicKey, outKeyPath);

    // Assert
    Assert.Equal(key.ToString(), keyFromFile.ToString());

    // Cleanup
    File.Delete(outKeyPath);
  }
}
