namespace Devantler.SecretManager.Core;

/// <summary>
/// Represents errors that occur during secret manager operations.
/// </summary>
public class SecretManagerException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SecretManagerException"/> class.
  /// </summary>
  public SecretManagerException()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SecretManagerException"/> class with a specified error message.
  /// </summary>
  /// <param name="message"></param>
  public SecretManagerException(string? message) : base(message)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SecretManagerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message"></param>
  /// <param name="innerException"></param>
  public SecretManagerException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
