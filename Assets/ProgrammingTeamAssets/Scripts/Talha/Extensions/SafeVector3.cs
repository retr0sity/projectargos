using UnityEngine;

[System.Serializable]
public class SafeVector3
{
    public float X;
    public float Y;
    public float Z;

    public SafeVector3(Vector3 Vector3)
    {
        this.X = Vector3.x;
        this.Y = Vector3.y;
        this.Z = Vector3.z;
    }
}
