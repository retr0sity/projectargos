using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Game.Utilities
{
    public class ButtonRenderer : MonoBehaviour
    {
        public bool Interactable
        {
            get => Collider.enabled;
            set => Collider.enabled = value;
        }
        private Collider2D mCollider;
        private Collider2D Collider
        {
            get
            {
                mCollider = GetComponent<Collider2D>();
                if (mCollider == null)
                {
                    mCollider = gameObject.AddComponent<BoxCollider2D>();
                }
                return mCollider;
            }
        }

        public UnityEvent OnClickEvent
        {
            get;
            private set;
        } = new UnityEvent();

        private void OnMouseUpAsButton()
        {
            if (Interactable)
                OnClickEvent?.Invoke();
        }
    }
}