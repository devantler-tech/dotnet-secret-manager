using Devantler.KeyManager.Core.Models;
using Devantler.Keys.Core;

namespace Devantler.KeyManager.Core;

/// <summary>
/// Interface for a local key manager.
/// </summary>
public interface ILocalKeyManager<T> : IKeyManager<T> where T : IKey
{
    /// <summary>
    /// Import a key from a Key object to a specified key file.
    /// </summary>
    /// <param name="inKey"></param>
    /// <param name="outKeyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The imported <see cref="IKey"/>.</returns>
    Task<T> ImportKeyAsync(T inKey, string outKeyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Import a key from a file to a specified key file.
    /// </summary>
    /// <param name="inKeyPath"></param>
    /// <param name="inKeyPublicKey"></param>
    /// <param name="outKeyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The imported <see cref="IKey"/>.</returns>
    Task<T> ImportKeyAsync(string inKeyPath, string outKeyPath, string? inKeyPublicKey = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new key, and add it to a specified key file.
    /// </summary>
    /// <param name="outKeyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created <see cref="IKey"/>.</returns>
    Task<T> CreateKeyAsync(string outKeyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a key from a specified key file.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="keyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The deleted <see cref="IKey"/>.</returns>
    Task<T> DeleteKeyAsync(T key, string keyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a key by public key from a specified key file.
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="keyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The deleted <see cref="IKey"/>.</returns>
    Task<T> DeleteKeyAsync(string publicKey, string keyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a key by public key from a specified key file.
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="keyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="IKey"/>.</returns>
    Task<T> GetKeyAsync(string publicKey, string keyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// List all keys from a specified key file.
    /// </summary>
    /// <param name="keyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>An <see cref="IEnumerable{T}"/> of keys.</returns>
    Task<IEnumerable<T>> ListKeysAsync(string keyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a key exists by public key in a specified key file.
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="keyPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="bool"/> indicating whether the key exists or not.</returns>
    Task<bool> KeyExistsAsync(string publicKey, string keyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a .sops.yaml file.
    /// </summary>
    /// <param name="configPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="SOPSConfig"/></returns>
    Task<SOPSConfig> GetSOPSConfigAsync(string configPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a .sops.yaml file.
    /// </summary>
    /// <param name="configPath"></param>
    /// <param name="config"></param>
    /// <param name="overwrite"></param>
    /// <param name="cancellationToken"></param>
    Task CreateSOPSConfigAsync(string configPath, SOPSConfig config, bool overwrite = false, CancellationToken cancellationToken = default);
}
