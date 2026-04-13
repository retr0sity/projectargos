using UnityEngine;

public class CameraFollowComponent : MonoBehaviour
{
    public float FollowSpeed = 5;

    private bool CanFollow;
    public Transform Player;
    private Vector3 StartPosition;
    private Vector3 TargetPosition;
    private void Start()
    {
        StartPosition = transform.position;
    }
    private void LateUpdate()
    {
        if (!CanFollow)
            return;

        TargetPosition = Player.transform.position;
        TargetPosition.z = StartPosition.z;
        transform.position = Vector3.Lerp(transform.position,TargetPosition,FollowSpeed * Time.deltaTime);
    }

    public void SetFollowStatus(bool Status)
    {
        CanFollow = Status;
        if (!CanFollow)
            transform.position = StartPosition;
    }
}
