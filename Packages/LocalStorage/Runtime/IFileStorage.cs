#if !DISABLE_UNITASK_SUPPORT && UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace LocalStorage
{
    public interface IFileStorage
    {
        void Save<TData>(TData data, string fileName);

        TData Load<TData>(string fileName);

        bool Delete(string fileName);

        string GetFilePath(string fileName);

        bool FileExists(string fileName);
    }

    public interface IFileStorageAsync : IFileStorage
    {
        #if !DISABLE_UNITASK_SUPPORT && UNITASK_SUPPORT
        UniTask SaveAsync<TData>(TData data, string fileName);
        #else
        Task SaveAsync<TData>(TData data, string fileName);
        #endif

        #if !DISABLE_UNITASK_SUPPORT && UNITASK_SUPPORT
        UniTask<TData> LoadAsync<TData>(string fileName);
        #else
        Task<TData> LoadAsync<TData>(string fileName);
        #endif
    }
}