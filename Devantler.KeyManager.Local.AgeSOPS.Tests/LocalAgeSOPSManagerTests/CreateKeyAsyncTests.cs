namespace Devantler.KeyManager.Local.AgeSOPS.Tests;

public class CreateKeyAsyncTests
{
  readonly LocalAgeSOPSKeyManager keyManager = new();
  [Fact]
  public async Task CreateKeyAsync_GivenNoOutKeyPath_CreatesKeyInSOPSKeyFile()
  {
    // Act
    var key = await keyManager.CreateKeyAsync();
    var keyFromFile = await keyManager.GetKeyAsync(key.PublicKey);

    // Assert
    Assert.Equal(key.ToString(), keyFromFile.ToString());
  }
}
