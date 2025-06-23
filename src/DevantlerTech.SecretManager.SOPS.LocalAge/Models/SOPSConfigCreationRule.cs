using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DevantlerTech.SecretManager.SOPS.LocalAge.Models;

/// <summary>
/// Represents a Creation Rule in a .sops.yaml file.
/// </summary>
public class SOPSConfigCreationRule
{
  /// <summary>
  /// The path regex for the files that is managed by this rule.
  /// </summary>
  public required string PathRegex { get; set; }

  /// <summary>
  /// The encrypted regex for the fields that is managed by this rule.
  /// </summary>
  public string EncryptedRegex { get; set; } = "^(data|stringData)$";

  /// <summary>
  /// The public keys that can manage the files that is managed by this rule.
  /// </summary>
  [YamlMember(ScalarStyle = ScalarStyle.Literal)]
  public required string Age { get; set; }
}
