namespace Devantler.KeyManager.Core;

/// <summary>
/// Represents errors that occur during key manager operations.
/// </summary>
public class KeyManagerException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="KeyManagerException"/> class.
  /// </summary>
  public KeyManagerException()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="KeyManagerException"/> class with a specified error message.
  /// </summary>
  /// <param name="message"></param>
  public KeyManagerException(string? message) : base(message)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="KeyManagerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message"></param>
  /// <param name="innerException"></param>
  public KeyManagerException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
