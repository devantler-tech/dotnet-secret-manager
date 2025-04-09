namespace Devantler.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.CreateKeyAsync(CancellationToken)"/>.
/// </summary>
public class CreateKeyAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.CreateKeyAsync(CancellationToken)"/> creates a key in the SOPS age key file.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task CreateKeyAsync_CreatesKeyInSOPSKeyFile()
  {
    // Act
    var key = await _secretManager.CreateKeyAsync();
    var keyFromFile = await _secretManager.GetKeyAsync(key.PublicKey);

    // Assert
    Assert.Equal(key.ToString(), keyFromFile.ToString());

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(key);
  }
}
