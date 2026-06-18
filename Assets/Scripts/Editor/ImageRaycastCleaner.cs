#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace WheelDemo.EditorTools
{
    // Disables RaycastTarget and the Maskable flag on every Image in the open
    // scene that is not on a Button/Toggle/etc. Run from the menu before a build.
    public static class ImageRaycastCleaner
    {
        [MenuItem("WheelDemo/Clean Decorative Image Flags")]
        private static void Clean()
        {
            int touched = 0;
            foreach (var img in Object.FindObjectsOfType<Image>(true))
            {
                bool interactive = img.GetComponent<Selectable>() != null
                                   || img.GetComponentInParent<Selectable>() != null;
                if (interactive) continue;

                if (img.raycastTarget || img.maskable)
                {
                    Undo.RecordObject(img, "Clean image flags");
                    img.raycastTarget = false;
                    img.maskable = false;
                    EditorUtility.SetDirty(img);
                    touched++;
                }
            }
            Debug.Log($"Cleaned {touched} decorative Image component(s).");
        }
    }
}
#endif
