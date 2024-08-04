namespace Devantler.SOPSManager.Core;

interface ISOPSManager
{
  /// <summary>
  /// Import a key from a Key object to the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="inKey"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task ImportKeyAsync(KeyType keyType, Key inKey, CancellationToken token = default);

  /// <summary>
  /// Import a key from a file to the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="inKeyPath"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task ImportKeyAsync(KeyType keyType, string inKeyPath, CancellationToken token = default);

  /// <summary>
  /// Import a key from one file to another, and optionally add it to the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="inKeyPath"></param>
  /// <param name="outKeyPath"></param>
  /// <param name="addToSops"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task ImportKeyToFileAsync(KeyType keyType, string inKeyPath, string outKeyPath, bool addToSops = true, CancellationToken token = default);

  /// <summary>
  /// Import a key from a Key object to a file, and optionally add it to the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="inKey"></param>
  /// <param name="outKeyPath"></param>
  /// <param name="addToSops"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task ImportKeyToFileAsync(KeyType keyType, Key inKey, string outKeyPath, bool addToSops = true, CancellationToken token = default);

  /// <summary>
  /// Create a new key, and add it to the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task CreateKeyAsync(KeyType keyType, CancellationToken token = default);

  /// <summary>
  /// Create a new key to a file, and optionally add it to the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="outKeyPath"></param>
  /// <param name="addToSops"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task CreateKeyToFileAsync(KeyType keyType, string outKeyPath, bool addToSops = true, CancellationToken token = default);

  /// <summary>
  /// Delete a key by public key from the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task DeleteKeyAsync(KeyType keyType, string publicKey, CancellationToken token = default);

  /// <summary>
  /// Delete a key, and optionally remove it from the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="keyPath"></param>
  /// <param name="removeFromSops"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task DeleteKeyFromFileAsync(KeyType keyType, string keyPath, bool removeFromSops = true, CancellationToken token = default);

  /// <summary>
  /// Delete a key by public key from a file, and optionally remove it from the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="keyPath"></param>
  /// <param name="publicKey"></param>
  /// <param name="removeFromSops"></param>
  /// <param name="token"></param>
  Task DeleteKeyFromFileAsync(KeyType keyType, string keyPath, string publicKey, bool removeFromSops = true, CancellationToken token = default);

  /// <summary>
  /// Get the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<string> GetSOPSKeyFileAsync(KeyType keyType, CancellationToken token = default);

  /// <summary>
  /// Get a key by public key from the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<Key> GetKeyAsync(KeyType keyType, string publicKey, CancellationToken token = default);

  /// <summary>
  /// Get a key from a file.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="keyPath"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<Key> GetKeyFromFileAsync(KeyType keyType, string keyPath, CancellationToken token = default);

  /// <summary>
  /// Get a key by public key from a file.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="keyPath"></param>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<Key> GetKeyFromFileAsync(KeyType keyType, string keyPath, string publicKey, CancellationToken token = default);

  /// <summary>
  /// List all keys from the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<IEnumerable<Key>> ListKeysAsync(KeyType keyType, CancellationToken token = default);

  /// <summary>
  /// List all keys from a file.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="keyPath"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<IEnumerable<Key>> ListKeysFromFileAsync(KeyType keyType, string keyPath, CancellationToken token = default);

  /// <summary>
  /// Check if a key exists by public key in the current SOPS_*_KEY_FILE.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<bool> KeyExistsAsync(KeyType keyType, string publicKey, CancellationToken token = default);

  /// <summary>
  /// Check if a key exists from a file.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="keyPath"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<bool> KeyExistsFromFileAsync(KeyType keyType, string keyPath, CancellationToken token = default);

  /// <summary>
  /// Check if a key exists by public key from a file.
  /// </summary>
  /// <param name="keyType"></param>
  /// <param name="keyPath"></param>
  /// <param name="publicKey"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  Task<bool> KeyExistsFromFileAsync(KeyType keyType, string keyPath, string publicKey, CancellationToken token = default);
}
