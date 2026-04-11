using UnityEngine;

namespace Game.Interface
{
    public interface IDraggable
    {
        void OnDragStart(Vector2 Position);
        void OnDragging(Vector2 Position);
        void OnDragEnd();
    }
}