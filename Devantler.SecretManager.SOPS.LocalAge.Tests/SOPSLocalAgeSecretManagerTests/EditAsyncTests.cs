namespace Devantler.SecretManager.SOPS.LocalAge.Tests.SOPSLocalAgeSecretManagerTests;

/// <summary>
/// Tests for <see cref="SOPSLocalAgeSecretManager.EncryptAsync(string, string, CancellationToken)"/>.
/// </summary>
[Collection("SOPSLocalAgeSecretManager")]
public class EditAsyncTests
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
    string filePath = Path.GetTempPath() + "edit-async-test.yaml";
    File.WriteAllText(filePath, plainText);

    // Act
    string encryptedText = await _secretManager.EncryptAsync(filePath, key.PublicKey);
    async Task task() => await _secretManager.EditAsync(filePath);

    // Assert
    Assert.NotEqual(plainText, encryptedText);
    var exception = await Record.ExceptionAsync(async () => await Task.WhenAny(task(), Task.Delay(5000)));
    Assert.Null(exception);

    // Cleanup
    _ = await _secretManager.DeleteKeyAsync(key);
  }
}

