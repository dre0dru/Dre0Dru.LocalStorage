using System;
#if !DISABLE_UNITASK_SUPPORT && UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using UnityEngine.Scripting;

namespace LocalStorage.Providers
{
    public class CombinedDataTransform : IDataTransformAsync
    {
        private readonly IDataTransformAsync _firstTransform;
        private readonly IDataTransformAsync _secondTransform;

        [RequiredMember]
        public CombinedDataTransform(IDataTransformAsync firstTransform, IDataTransformAsync secondTransform)
        {
            _firstTransform = firstTransform ??
                              throw new ArgumentNullException(nameof(firstTransform));
            _secondTransform = secondTransform ??
                               throw new ArgumentNullException(nameof(secondTransform));
        }

        public byte[] Apply(byte[] data) =>
            _secondTransform.Apply(_firstTransform.Apply(data));

        #if !DISABLE_UNITASK_SUPPORT && UNITASK_SUPPORT
        public UniTask<byte[]> ApplyAsync(byte[] data) =>
            _firstTransform.ApplyAsync(data)
                .ContinueWith(bytes => _secondTransform.ApplyAsync(bytes));
        #else
        public Task<byte[]> ApplyAsync(byte[] data) =>
            _firstTransform.ApplyAsync(data)
                .ContinueWith(task => _secondTransform.ApplyAsync(task.Result))
                .Unwrap();
        #endif

        public byte[] Reverse(byte[] data) =>
            _firstTransform.Reverse(_secondTransform.Reverse(data));

        #if !DISABLE_UNITASK_SUPPORT && UNITASK_SUPPORT
        public UniTask<byte[]> ReverseAsync(byte[] data) =>
            _secondTransform.ReverseAsync(data)
                .ContinueWith(bytes => _firstTransform.ReverseAsync(bytes));
        #else
        public Task<byte[]> ReverseAsync(byte[] data) =>
            _secondTransform.ReverseAsync(data)
                .ContinueWith(task => _firstTransform.ReverseAsync(task.Result))
                .Unwrap();
        #endif
    }
}