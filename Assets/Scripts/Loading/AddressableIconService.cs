using System;
using System.Collections.Generic;
using UnityEngine;
#if ADDRESSABLES_PRESENT
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace WheelDemo.Loading
{
    // Thin façade over Addressables for loading reward icon sprites by key.
    //
    // The public API takes a plain string key and returns the sprite through a
    // callback, so callers never touch Addressables types. The Addressables
    // implementation is compiled only when the package is installed
    // (ADDRESSABLES_PRESENT is set by this assembly's versionDefine). Until then
    // the no-op fallback is used, and callers keep showing their embedded icon,
    // so the game runs identically with or without an Addressables content build.
    public static class AddressableIconService
    {
        // True when the Addressables package is installed and this path is live.
        public static bool IsAvailable =>
#if ADDRESSABLES_PRESENT
            true;
#else
            false;
#endif

#if ADDRESSABLES_PRESENT
        private static readonly Dictionary<string, AsyncOperationHandle<Sprite>> handles = new();
#endif

        /// <summary>
        /// Loads the sprite at <paramref name="key"/> and invokes
        /// <paramref name="onLoaded"/> with it (or null on failure / when the
        /// package is not installed). Handles are cached and reused per key.
        /// </summary>
        public static void LoadIcon(string key, Action<Sprite> onLoaded)
        {
            if (string.IsNullOrEmpty(key))
            {
                onLoaded?.Invoke(null);
                return;
            }

#if ADDRESSABLES_PRESENT
            if (handles.TryGetValue(key, out var existing))
            {
                if (existing.IsDone) onLoaded?.Invoke(existing.Result);
                else existing.Completed += h => onLoaded?.Invoke(h.Status == AsyncOperationStatus.Succeeded ? h.Result : null);
                return;
            }

            var handle = Addressables.LoadAssetAsync<Sprite>(key);
            handles[key] = handle;
            handle.Completed += h =>
                onLoaded?.Invoke(h.Status == AsyncOperationStatus.Succeeded ? h.Result : null);
#else
            onLoaded?.Invoke(null);
#endif
        }

        // Releases every cached handle. Call on teardown to free Addressable assets.
        public static void ReleaseAll()
        {
#if ADDRESSABLES_PRESENT
            foreach (var handle in handles.Values)
                if (handle.IsValid()) Addressables.Release(handle);
            handles.Clear();
#endif
        }
    }
}
