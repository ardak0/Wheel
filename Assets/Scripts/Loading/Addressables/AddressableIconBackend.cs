#if ADDRESSABLES_PRESENT
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using WheelDemo.Loading;

namespace WheelDemo.Loading.Addressables
{
    // Addressables-backed implementation of IconLoader's backend. This whole
    // assembly only compiles when the Addressables package is installed (the
    // asmdef's ADDRESSABLES_PRESENT define constraint), so gameplay code can
    // depend on the IconLoader facade without depending on the package.
    public static class AddressableIconBackend
    {
        private static readonly Dictionary<string, AsyncOperationHandle<Sprite>> handles = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register() => IconLoader.RegisterBackend(Load);

        private static void Load(string key, Action<Sprite> onLoaded)
        {
            if (handles.TryGetValue(key, out var existing))
            {
                if (existing.IsDone) onLoaded?.Invoke(existing.Result);
                else existing.Completed += h =>
                    onLoaded?.Invoke(h.Status == AsyncOperationStatus.Succeeded ? h.Result : null);
                return;
            }

            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Sprite>(key);
            handles[key] = handle;
            handle.Completed += h =>
                onLoaded?.Invoke(h.Status == AsyncOperationStatus.Succeeded ? h.Result : null);
        }
    }
}
#endif
