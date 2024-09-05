# 🔓 .NET Key Manager

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Test](https://github.com/devantler/dotnet-key-manager/actions/workflows/test.yaml/badge.svg)](https://github.com/devantler/dotnet-key-manager/actions/workflows/test.yaml)
[![codecov](https://codecov.io/gh/devantler/dotnet-key-manager/graph/badge.svg?token=RhQPb4fE7z)](https://codecov.io/gh/devantler/dotnet-key-manager)

A simple .NET library to manage cryptographic keys.

<details>
  <summary>Show/hide folder structure</summary>

<!-- readme-tree start -->

```
.
├── .github
│   └── workflows
├── Devantler.Keys.Age
│   └── Utils
├── Devantler.Keys.Age.Tests
│   ├── AgeKeyTests
│   └── Utils
│       └── DateTimeFormatterTests
└── Devantler.Keys.Core

9 directories
```

<!-- readme-tree end -->

</details>

## Prerequisites

- [.NET](https://dotnet.microsoft.com/en-us/)

## 🚀 Getting Started

To get started, you can install the packages from NuGet.

```bash
# For the Age key model
dotnet add package Devantler.KeyManager.Local.Age
```

If you need to create a new implementation for a key manager, you can install the core package.

```bash
dotnet add package Devantler.Keys.Core
```

## Usage

### Local Age Key Manager

The Local Age Key Manager is a simple key manager to manage Age keys on your local machine. The key manager saves and loads keys from your SOPS keyring by default.

#### Create a new key

To create a new key, you can use the `CreateKeyAsync` method.

```csharp
using Devantler.KeyManager.Local.Age;

var keyManager = new LocalAgeKeyManager();

var key = await keyManager.CreateKeyAsync();
```

To delete a key, you can use the `DeleteKeyAsync` method.

```csharp
using Devantler.Keys.Age;
using Devantler.KeyManager.Local.Age;

var keyManager = new LocalAgeKeyManager();

var ageKey = AgeKeygen.InMemory();
await keyManager.DeleteKeyAsync(ageKey);
```

#### Get an existing key

To get an existing key, you can use the `GetKeyAsync` method.

```csharp
using Devantler.KeyManager.Local.Age;

var keyManager = new LocalAgeKeyManager();

var key = await keyManager.GetKeyAsync("<public key>");
```

#### Import a key

To import a key, you can use the `ImportKeyAsync` method.

```csharp
using Devantler.Keys.Age;
using Devantler.KeyManager.Local.Age;

var keyManager = new LocalAgeKeyManager();

var ageKey = AgeKeygen.InMemory();

var key = await keyManager.ImportKeyAsync(ageKey);
```

#### Check if a key exists

To check if a key exists, you can use the `KeyExistsAsync` method.

```csharp
using Devantler.KeyManager.Local.Age;

var keyManager = new LocalAgeKeyManager();

var exists = await keyManager.KeyExistsAsync("<public key>");
```

#### List all keys

To list all keys, you can use the `ListKeysAsync` method.

```csharp
using Devantler.KeyManager.Local.Age;

var keyManager = new LocalAgeKeyManager();

var keys = await keyManager.ListKeysAsync();
```
