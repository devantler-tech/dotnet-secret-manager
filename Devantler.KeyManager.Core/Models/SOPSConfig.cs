#pragma warning disable CA2227
using System.Collections.ObjectModel;

namespace Devantler.KeyManager.Core.Models;

/// <summary>
/// Represents a .sops.yaml file.
/// </summary>
public class SOPSConfig
{
  /// <summary>
  /// A list of creation rules.
  /// </summary>
  public required Collection<SOPSConfigCreationRule> CreationRules { get; set; } = [];
}


