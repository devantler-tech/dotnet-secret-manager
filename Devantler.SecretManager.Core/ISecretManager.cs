using Devantler.Keys.Core;

namespace Devantler.SecretManager.Core;

/// <summary>
/// Interface for a secret manager.
/// </summary>
public interface ISecretManager<T> where T : IKey
{
  /// <summary>
  /// Import a key from a Key object.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>The imported <see cref="IKey"/>.</returns>
  Task<T> ImportKeyAsync(T key, CancellationToken cancellationToken = default);

  /// <summary>
  /// Create a new key.
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>The created <see cref="IKey"/>.</returns>
  Task<T> CreateKeyAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Delete a key.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>The deleted <see cref="IKey"/>.</returns>
  Task<T> DeleteKeyAsync(T key, CancellationToken cancellationToken = default);

  /// <summary>
  /// Delete a key by public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>The deleted <see cref="IKey"/>.</returns>
  Task<T> DeleteKeyAsync(string publicKey, CancellationToken cancellationToken = default);

  /// <summary>
  /// Get a key by public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="IKey"/>.</returns>
  Task<T> GetKeyAsync(string publicKey, CancellationToken cancellationToken = default);

  /// <summary>
  /// List all keys.
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns>An <see cref="IEnumerable{T}"/> of keys.</returns>
  Task<IEnumerable<T>> ListKeysAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Check if a key exists by public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>A <see cref="bool"/> indicating whether the key exists or not.</returns>
  Task<bool> KeyExistsAsync(string publicKey, CancellationToken cancellationToken = default);

  /// <summary>
  /// Encrypt a file.
  /// </summary>
  /// <param name="filePath"></param>
  /// <param name="publicKey"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<string> EncryptAsync(string filePath, string publicKey, CancellationToken cancellationToken = default);

  /// <summary>
  /// Decrypt a file.
  /// </summary>
  /// <param name="filePath"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<string> DecryptAsync(string filePath, CancellationToken cancellationToken = default);
}
