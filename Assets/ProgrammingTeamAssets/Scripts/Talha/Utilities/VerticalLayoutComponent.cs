using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    [ExecuteInEditMode]
    public class VerticalLayoutComponent : MonoBehaviour
    {
        private List<Transform> Transforms;
        public float Spacing;

        private Vector3 StartPosition;
        private bool UpdateMode = true;

        [Utilities.Button]
        public void SwitchUpdateMode()
        {
            UpdateMode = !UpdateMode;
        }

        private void Update()
        {
            if (!UpdateMode)
                return;
            if (Transforms == null)
            {
                Transforms = new List<Transform>();
            }
            //if (transform.childCount != Transforms.Count)
            {
                Transforms.Clear();
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeInHierarchy)
                        Transforms.Add(transform.GetChild(i));
                }
            }
            if (Transforms.Count > 0)
            {
                var StartPos = (Spacing * (Transforms.Count - 1) / 2);
                for (int i = 0; i < Transforms.Count; i++)
                {
                    StartPosition.y = StartPos - (i * Spacing);
                    Transforms[i].localPosition = StartPosition;
                }
            }
        }
    }
}