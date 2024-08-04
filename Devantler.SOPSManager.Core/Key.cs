namespace Devantler.SOPSManager.Core;

public class Key
{
  public string Raw { get; set; } = string.Empty;
  public string Path { get; set; } = string.Empty;
  public required string PublicKey { get; set; }
  public required string PrivateKey { get; set; }
}
