using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Interface;

namespace Game.Components
{
    public abstract class BaseDraggableComponent : MonoBehaviour, IDraggable
    {
        public bool IsInteractable
        {
            get => Collider2D.enabled;
            set => Collider2D.enabled = value;
        }

        private Collider2D mCollider2D;
        private Collider2D Collider2D
        {
            get
            {
                if(mCollider2D == null)
                {
                    mCollider2D = GetComponent<Collider2D>();
                }
                return mCollider2D;
            }
        }

        protected Vector2 Offset = Vector2.zero;

        public virtual void OnDragEnd()
        {
            Debug.Log("OnDragEnd");
            //gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
        }

        public virtual void OnDragging(Vector2 Position)
        {
            //Debug.Log("OnDragging");
            gameObject.transform.position = Position + Offset;
        }

        public virtual void OnDragStart(Vector2 Position)
        {
            Offset = new Vector2(gameObject.transform.position.x - Position.x, gameObject.transform.position.y - Position.y);
            Debug.Log("OnDragStart");
        }
    }
}