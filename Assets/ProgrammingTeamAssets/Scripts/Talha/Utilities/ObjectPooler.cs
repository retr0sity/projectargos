using System;
using System.Collections;
using System.Collections.Generic;
using Game.Extensions;
using UnityEngine;
using System.Linq;

namespace Game.Utilities
{
    public class ObjectPooler<T> where T : MonoBehaviour
    {
        private T Preset;
        private List<T> Pool;

        public ObjectPooler(T Preset)
        {
            this.Preset = Preset;
            Pool = new List<T>();
        }

        public T GetInstance(int Index)
        {
            T Component;
            if (Index < Pool.Count)
            {
                Component = Pool[Index];
            }
            else
            {
                Component = GenerateComponent();

            }
            ResetTransform(Component.transform);
            return Component;
        }

        public T GetInstance()
        {
            T Component = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            if (Component == null)
            {
                Component = GenerateComponent();
            }
            Pool.Remove(Component);
            ResetTransform(Component.transform);
            Pool.Add(Component);
            return Component;
        }

        public void DisableAll()
        {
            for (int i = 0; i < Pool.Count; i++)
            {
                Pool[i].gameObject.SetActive(false);
            }
        }

        private T GenerateComponent()
        {
            var Component = MonoBehaviour.Instantiate(Preset, Preset.transform.parent);
            Pool.Add(Component);
            Component.name = Preset.gameObject.name + Pool.Count.ToString();
            return Component;
        }

        private void ResetTransform(Transform Transform)
        {
            Transform.SetAsLastSibling();
            Transform.ResetLocalTransform();
            Transform.gameObject.SetActive(true);
        }

        public int Count()
        {
            return Pool.Count;
        }

        public T GetItemAtIndex(int Index)
        {
            return Pool[Index];
        }
    }
}