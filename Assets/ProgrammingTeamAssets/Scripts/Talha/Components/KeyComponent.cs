using DG.Tweening;
using UnityEngine;

namespace Game.Components
{
    public class KeyComponent : MonoBehaviour
    {
        public bool IsCollected;

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

        public void CollectKey()
        {
            IsCollected = true;
            Collider2D.enabled = false;
            GetComponentInParent<DoorGameplayComponent>().ActionKeyCollected?.Invoke();
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive((false)));
        }
    }
}
