using System.Globalization;
using Devantler.AgeCLI;
using Devantler.Keys.Age;

namespace Devantler.SecretManager.SOPS.LocalAge.Utils;

/// <summary>
/// Helper class to run common agekeygen commands.
/// </summary>
public static class AgeKeygenHelper
{
  /// <summary>
  /// Create a new Age key.
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static async Task<AgeKey> CreateAgeKeyAsync(CancellationToken cancellationToken = default)
  {
    var (exitCode, output) = await AgeKeygen.RunAsync(
      [],
      silent: true,
      cancellationToken: cancellationToken).ConfigureAwait(false);
    if (exitCode != 0)
      throw new InvalidOperationException($"Failed to generate key: {output}");
    string[] lines = output.Split("\n");
    var createdAt = DateTime.Parse(lines[0].Split(" ")[2], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
    string publicKey = lines[1].Split(" ")[3];
    string privateKey = lines[2];
    var ageKey = new AgeKey(
      publicKey,
      privateKey,
      createdAt
    );
    return ageKey;
  }
}
