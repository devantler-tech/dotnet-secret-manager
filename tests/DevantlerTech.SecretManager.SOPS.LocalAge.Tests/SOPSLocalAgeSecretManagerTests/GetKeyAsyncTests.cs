namespace DevantlerTech.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.GetKeyAsync(string, CancellationToken)"/>.
/// </summary>
public class GetKeyAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.GetKeyAsync(string, CancellationToken)"/> gets a key from the SOPS key file.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetKeyAsync_GivenPublicKey_ReturnsKeyFromSOPSKeyFile()
  {
    // Arrange
    var key = await _secretManager.CreateKeyAsync();

    // Act
    var keyFromFile = await _secretManager.GetKeyAsync(key.PublicKey);

    // Assert
    Assert.Equal(key.ToString(), keyFromFile.ToString());

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(key);
  }
}
