namespace DevantlerTech.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.ListKeysAsync(CancellationToken)"/>.
/// </summary>
public class ListKeysAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.ListKeysAsync(CancellationToken)"/> returns keys from the SOPS key file.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ListKeysAsync_ReturnsKeysFromSOPSKeyFile()
  {
    // Arrange
    var key1 = await _secretManager.CreateKeyAsync();
    var key2 = await _secretManager.CreateKeyAsync();

    // Act
    var keys = await _secretManager.ListKeysAsync();

    // Assert
    Assert.Contains(keys, k => k.PublicKey == key1.PublicKey && k.PrivateKey == key1.PrivateKey && k.CreatedAt == key1.CreatedAt && k.ToString() == key1.ToString());
    Assert.Contains(keys, k => k.PublicKey == key2.PublicKey && k.PrivateKey == key2.PrivateKey && k.CreatedAt == key2.CreatedAt && k.ToString() == key2.ToString());

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(key1);
    _ = await _secretManager.DeleteKeyAsync(key2);
  }
}
