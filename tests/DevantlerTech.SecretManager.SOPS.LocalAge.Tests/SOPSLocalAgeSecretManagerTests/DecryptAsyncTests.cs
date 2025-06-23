namespace DevantlerTech.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.DecryptAsync(string, CancellationToken)"/>.
/// </summary>
public class DecryptAsyncTests
{
  readonly SOPSLocalAgeSecretManager _secretManager = new();

  /// <summary>
  /// Tests that <see cref="SOPSLocalAgeSecretManager.DecryptAsync(string, CancellationToken)"/> decrypts a file with a private key.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task DecryptAsync_GivenPathAndPublicKey_ReturnsDecryptedString()
  {
    // Arrange
    var key = await _secretManager.CreateKeyAsync();
    string plainText = "secret: AGE-SECRET-KEY-1VZQ";
    string filePath = Path.GetTempPath() + "decrypt-async-test.yaml";
    await File.WriteAllTextAsync(filePath, plainText);

    // Act
    string encryptedText = await _secretManager.EncryptAsync(filePath, key.PublicKey);
    await File.WriteAllTextAsync(filePath, encryptedText);
    string decryptedText = await _secretManager.DecryptAsync(filePath);

    // Assert
    Assert.NotEqual(plainText, encryptedText);
    Assert.NotEqual(encryptedText, decryptedText);
    // ignore whitespace and newlines
    Assert.Equal(plainText.TrimEnd(), decryptedText.TrimEnd());

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(key);
  }
}

