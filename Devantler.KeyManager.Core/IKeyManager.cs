using Devantler.Keys.Core;

namespace Devantler.KeyManager.Core;

/// <summary>
/// Interface for a key manager.
/// </summary>
public interface IKeyManager<T> where T : IKey
{
  /// <summary>
  /// Import a key from a Key object.
  /// </summary>
  /// <param name="inKey"></param>
  /// <param name="token"></param>
  /// <returns>The imported <see cref="IKey"/>.</returns>
  Task<T> ImportKeyAsync(T inKey, CancellationToken token = default);

  /// <summary>
  /// Import a key from a file.
  /// </summary>
  /// <param name="inKeyPath"></param>
  /// <param name="inKeyPublicKey"></param>
  /// <param name="token"></param>
  /// <returns>The imported <see cref="IKey"/>.</returns>
  Task<T> ImportKeyAsync(string inKeyPath, string? inKeyPublicKey = default, CancellationToken token = default);

  /// <summary>
  /// Create a new key.
  /// </summary>
  /// <param name="token"></param>
  /// <returns>The created <see cref="IKey"/>.</returns>
  Task<T> CreateKeyAsync(CancellationToken token = default);

  /// <summary>
  /// Delete a key.
  /// </summary>
  /// <param name="key"></param>
  /// <param name="token"></param>
  /// <returns>The deleted <see cref="IKey"/>.</returns>
  Task<T> DeleteKeyAsync(T key, CancellationToken token = default);

  /// <summary>
  /// Delete a key by public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns>The deleted <see cref="IKey"/>.</returns>
  Task<T> DeleteKeyAsync(string publicKey, CancellationToken token = default);

  /// <summary>
  /// Get a key by public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns>A <see cref="IKey"/>.</returns>
  Task<T> GetKeyAsync(string publicKey, CancellationToken token = default);

  /// <summary>
  /// List all keys.
  /// </summary>
  /// <param name="token"></param>
  /// <returns>An <see cref="IEnumerable{T}"/> of keys.</returns>
  Task<IEnumerable<T>> ListKeysAsync(CancellationToken token = default);

  /// <summary>
  /// Check if a key exists by public key.
  /// </summary>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns>A <see cref="bool"/> indicating whether the key exists or not.</returns>
  Task<bool> KeyExistsAsync(string publicKey, CancellationToken token = default);
}
