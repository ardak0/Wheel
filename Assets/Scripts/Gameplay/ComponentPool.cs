using System.Collections.Generic;
using UnityEngine;

namespace WheelDemo.Gameplay
{
    // Minimal reuse pool for prefab-instantiated Components. Avoids the
    // per-zone Instantiate/Destroy churn (and the GC/allocation spikes that
    // come with it) by deactivating instances and handing them back out.
    public class ComponentPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Stack<T> idle = new();
        private readonly List<T> active = new();

        public ComponentPool(T prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        public IReadOnlyList<T> Active => active;

        public T Get()
        {
            T item = idle.Count > 0 ? idle.Pop() : Object.Instantiate(prefab, parent);
            item.gameObject.SetActive(true);
            active.Add(item);
            return item;
        }

        public void Release(T item)
        {
            if (item == null) return;
            item.gameObject.SetActive(false);
            active.Remove(item);
            idle.Push(item);
        }

        // Returns every live instance to the pool without destroying anything.
        public void ReleaseAll()
        {
            for (int i = 0; i < active.Count; i++)
            {
                if (active[i] != null) active[i].gameObject.SetActive(false);
                if (active[i] != null) idle.Push(active[i]);
            }
            active.Clear();
        }
    }
}
