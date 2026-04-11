using UnityEngine;

namespace Game.Interface
{
    public interface ITransformProvider
    {
        Transform Check(Ray Ray);
    }
}