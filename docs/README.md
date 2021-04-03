# Description
Configurable generic class for managing local data saved on device.
Unity 2020.1+

## Features
- One generic class with configuration options for data read/write and serialization/deserialization.
- Simple file provider that just reads/writes data.
- Encrypted file provider that uses AES encryption to preprocess data.
- Json serialization provider that uses JsonUtility for serialization.
- Options to create your own file/serialization providers.
- Saves files to `Application.persistentDataPath`.
- Async/sync API.

# Installation
This package can be installed as unity module directly from git url in two ways:
- By adding following line in `Packages/manifest.json`:
  ```
  "com.dre0dru.localstorage": "https://github.com/dre0dru/LocalStorage.git#upm",
  ```
- By using `Window/Package Manager/Add package from git URL...` in Unity:
  ```
  https://github.com/dre0dru/LocalStorage.git#upm
  ```
  
# Usage
## Common usage
```c#
//Serialization/deserialization implementation
ISerializationProvider serializationProvider = new UnityJsonSerializationProvider();

//Path to save/load from (optional)
string path = "dataFolder";  //Resulting path will be Application.persistentDataPath/dataFolder
//File save/load implementation
IFileProvider fileProvider = new FileProvider(path);

var storage = new Storage(serializationProvider, fileProvider);

string fileName = "fileName.extension";

//Resulting path will be Application.persistentDataPath/dataFolder/fileName.extension
string filePath = storage.GetFilePath(fileName);

//Serializes data then saves file
storage.Save(new Vector2(1.0f, 1.0f), fileName);

//Async saving
await storage.SaveAsync(new Vector2(1.0f, 1.0f), fileName);

//Check if file exists
bool exists = storage.FileExists(fileName);

//Loads file then deserializes data
Vector2 deserialized = storage.Load<Vector2>(fileName);

//Async loading
Vector2 deserialized = await storage.LoadAsync<Vector2>(fileName);

//Deletes file if present
storage.Delete(fileName);
```
## Encrypted file provider usage
- Implement encryption settings:
```c#
//This is just an example, don't generate new key/IV every time
public class ExampleEncryptionSettings : IEncryptionSettings
{
    public byte[] Key { get; private set; }
    public byte[] InitializationVector { get; private set; }

    public ExampleEncryptionSettings()
    {
        //Create AES instance
        using var aes = Aes.Create();
        //Save AES key somewhere
        Key = aes.Key;
        //Save AES IV somewhere
        InitializationVector = aes.IV;
    }
}
```
- Use `EncryptedFileProvider`:
```c#
ISerializationProvider serializationProvider = new UnityJsonSerializationProvider();
IFileProvider fileProvider = new FileProvider();

//Encryption settings for AES
IEncryptionSettings encryptionSettings = new ExampleEncryptionSettings();

//File save/load implementation with encryption
//Just a wrapper around IFileProvider with encryption before save/after load
IFileProvider encryptedFileProvider = new EncryptedFileProvider(fileProvider, 
    new ExampleEncryptionSettings());

var storage = new Storage(serializationProvider, encryptedFileProvider);
//The rest as in common usage
```
## Generic storage usage
Best used with DI containers to bind `Storage` with different `ISerializationProvider`/`IFileProvider`:
```c#
ISerializationProvider jsonSerializationProvider = new UnityJsonSerializationProvider();
IFileProvider fileProvider = new FileProvider();

var jsonStorage = new Storage<UnityJsonSerializationProvider, FileProvider>(
    jsonSerializationProvider, fileProvider
);

IEncryptionSettings encryptionSettings = new ExampleEncryptionSettings();
IFileProvider encryptedFileProvider = new EncryptedFileProvider(fileProvider, 
    new ExampleEncryptionSettings());

var encryptedJsonStorage = new Storage<UnityJsonSerializationProvider, EncryptedFileProvider>(
    jsonSerializationProvider, encryptedFileProvider
);
```
# License
The software released under the terms of the [MIT license](./LICENSE.md).