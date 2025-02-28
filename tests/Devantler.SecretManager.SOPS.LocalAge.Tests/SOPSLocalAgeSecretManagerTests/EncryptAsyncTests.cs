namespace Devantler.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.EncryptAsync(string, string, CancellationToken)"/>.
/// </summary>
[Collection("SOPSLocalAgeSecretManager")]
public class EncryptAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.EncryptAsync(string, string, CancellationToken)"/> encrypts a file with a public key.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task EncryptAsync_GivenPathAndPublicKey_ReturnsEncryptedString()
  {
    // Arrange
    var key = await _secretManager.CreateKeyAsync();
    string plainText = """
      secret: |
        AGE-SECRET-KEY-1VZQ
    """;
    string filePath = Path.GetTempPath() + "encrypt-async-test.yaml";
    await File.WriteAllTextAsync(filePath, plainText);

    // Act
    string encryptedText = await _secretManager.EncryptAsync(filePath, key.PublicKey);

    // Assert
    Assert.NotEqual(plainText, encryptedText);

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(key);
  }
}

