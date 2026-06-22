using System;
using UnityEngine;

namespace WheelDemo.Loading
{
    // Always-compiled facade for loading reward icon sprites by key.
    //
    // This assembly has NO dependency on the Addressables package, so the project
    // compiles whether or not Addressables is installed. The actual loading
    // backend lives in the optional WheelDemo.Loading.Addressables assembly,
    // which compiles only when the package is present and registers itself here
    // at startup. With no backend registered, LoadIcon returns null and callers
    // fall back to their embedded sprite.
    public static class IconLoader
    {
        public delegate void LoadDelegate(string key, Action<Sprite> onLoaded);

        private static LoadDelegate backend;

        // True once an asset-loading backend (e.g. Addressables) has registered.
        public static bool HasBackend => backend != null;

        // Called by the optional backend assembly to plug itself in.
        public static void RegisterBackend(LoadDelegate loader) => backend = loader;

        public static void LoadIcon(string key, Action<Sprite> onLoaded)
        {
            if (backend != null && !string.IsNullOrEmpty(key)) backend(key, onLoaded);
            else onLoaded?.Invoke(null);
        }
    }
}
