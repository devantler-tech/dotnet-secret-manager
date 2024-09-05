using Devantler.AgeCLI;
using Devantler.Keys.Age;

namespace Devantler.KeyManager.Local.AgeSOPS.Tests.LocalAgeSOPSKeyManagerTests;

/// <summary>
/// Tests for <see cref="LocalAgeSOPSKeyManager.ImportKeyAsync(AgeKey, string?, CancellationToken)"/>.
/// </summary>
[Collection("LocalAgeSOPSKeyManager")]
public class ImportKeyAsyncTests
{
  readonly LocalAgeSOPSKeyManager keyManager = new();

  /// <summary>
  /// Tests that <see cref="LocalAgeSOPSKeyManager.ImportKeyAsync(AgeKey, string?, CancellationToken)"/> imports a key into the SOPS key file when no out key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ImportKeyAsync_GivenInKeyAndNoOutKeyPath_ImportsKeyIntoSOPSKeyFile()
  {
    // Arrange
    var inKey = await AgeKeygen.InMemory();

    // Act
    var outKey = await keyManager.ImportKeyAsync(inKey);
    var outKeyFromFile = await keyManager.GetKeyAsync(outKey.PublicKey);

    // Assert
    Assert.Equal(inKey.ToString(), outKeyFromFile.ToString());

    // Cleanup
    _ = await keyManager.DeleteKeyAsync(inKey);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeSOPSKeyManager.ImportKeyAsync(AgeKey, string?, CancellationToken)"/> imports a key into the specified key file when an out key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ImportKeyAsync_GivenInKeyAndOutKeyPath_ImportsKeyIntoSpecifiedKeyFile()
  {
    // Arrange
    var inKey = await AgeKeygen.InMemory();
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    // Act
    var outKey = await keyManager.ImportKeyAsync(inKey, outKeyPath);
    var outKeyFromFile = await keyManager.GetKeyAsync(outKey.PublicKey, outKeyPath);

    // Assert
    Assert.Equal(inKey.ToString(), outKeyFromFile.ToString());

    // Cleanup
    File.Delete(outKeyPath);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeSOPSKeyManager.ImportKeyAsync(AgeKey, string?, CancellationToken)"/> imports a key into the SOPS key file when no out key path is provided.
  /// </summary>
  [Fact]
  public async Task ImportKeyAsync_GivenInKeyPathAndNoOutKeyPath_ImportsKeyIntoSOPSKeyFile()
  {
    // Arrange
    var inKey = await AgeKeygen.InMemory();
    string inKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    await File.WriteAllTextAsync(inKeyPath, inKey.ToString());

    // Act
    var outKey = await keyManager.ImportKeyAsync(inKeyPath);
    var outKeyFromFile = await keyManager.GetKeyAsync(outKey.PublicKey);

    // Assert
    Assert.Equal(inKey.ToString(), outKeyFromFile.ToString());

    // Cleanup
    _ = await keyManager.DeleteKeyAsync(outKey);
    File.Delete(inKeyPath);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeSOPSKeyManager.ImportKeyAsync(AgeKey, string?, CancellationToken)"/> imports a key into the specified key file when an out key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ImportKeyAsync_GivenInKeyPathAndOutKeyPath_ImportsKeyIntoSpecifiedKeyFile()
  {
    // Arrange
    var inKey = await AgeKeygen.InMemory();
    string inKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    await File.WriteAllTextAsync(inKeyPath, inKey.ToString());
    string outKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    await File.WriteAllTextAsync(outKeyPath, "");

    // Act
    var outKey = await keyManager.ImportKeyAsync(inKeyPath, outKeyPath: outKeyPath);
    var outKeyFromFile = await keyManager.GetKeyAsync(outKey.PublicKey, outKeyPath);

    // Assert
    Assert.Equal(inKey.ToString(), outKeyFromFile.ToString());

    // Cleanup
    File.Delete(outKeyPath);
    File.Delete(inKeyPath);
  }

  /// <summary>
  /// Tests that <see cref="LocalAgeSOPSKeyManager.ImportKeyAsync(AgeKey, string?, CancellationToken)"/> imports a key into the SOPS key file when no out key path is provided.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task ImportKeyAsync_GivenInKeyPathAndInKeyPublicKeyAndNoOutKeyPath_ImportsKeyIntoSOPSKeyFile()
  {
    // Arrange
    var inKey1 = await AgeKeygen.InMemory();
    var inKey2 = await AgeKeygen.InMemory();
    string inKeyPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    await File.AppendAllLinesAsync(inKeyPath, [inKey1.ToString(), inKey2.ToString()]);

    // Act
    var outKey1 = await keyManager.ImportKeyAsync(inKeyPath, inKey1.PublicKey);
    var outKeyFromFile1 = await keyManager.GetKeyAsync(inKey1.PublicKey);
    var outKey2 = await keyManager.ImportKeyAsync(inKeyPath, inKey2.PublicKey);
    var outKeyFromFile2 = await keyManager.GetKeyAsync(inKey2.PublicKey);

    // Assert
    Assert.Equal(inKey1.ToString(), outKeyFromFile1.ToString());
    Assert.Equal(inKey2.ToString(), outKeyFromFile2.ToString());

    // Cleanup
    _ = await keyManager.DeleteKeyAsync(outKey1);
    _ = await keyManager.DeleteKeyAsync(outKey2);
    File.Delete(inKeyPath);
  }
}
