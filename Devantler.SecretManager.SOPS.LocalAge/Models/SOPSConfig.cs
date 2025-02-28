using System.Collections.ObjectModel;

namespace Devantler.SecretManager.SOPS.LocalAge.Models;

/// <summary>
/// Represents a .sops.yaml file.
/// </summary>
public class SOPSConfig
{
  /// <summary>
  /// A list of creation rules.
  /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
  public required Collection<SOPSConfigCreationRule> CreationRules { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
}
