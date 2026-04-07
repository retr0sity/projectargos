using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingRotation : MonoBehaviour
{
    private Transform MyTransform;
    public float Speed;

    private void Start()
    {
        ChangingEulars = transform.eulerAngles;
    }

    Vector3 ChangingEulars;
    private void FixedUpdate()
    {
        ChangingEulars.z -= Speed * Time.deltaTime;
        transform.localEulerAngles = ChangingEulars;  
    }
}
