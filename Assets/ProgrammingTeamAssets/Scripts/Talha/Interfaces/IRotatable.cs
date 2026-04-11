using UnityEngine;

namespace Game.Interface
{
    public interface IRotatable
    {
        void OnRotateStart(Vector2 Position);
        void OnRotating(Vector2 Position);
        void OnRotateEnd();
    }
}