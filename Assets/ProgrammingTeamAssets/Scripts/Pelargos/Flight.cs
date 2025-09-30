using UnityEngine;
using System.Collections;

public class Flight : MonoBehaviour
{
    [Header("Flying Settings")]
    public Vector3 flyDirection = new Vector3(0, 1, 1);
    public float flySpeed = 5f;
    public float flyDuration = 3f;

    private bool isFlying = false;

    private void Update()
    {
        if (isFlying)
            transform.Translate(flyDirection.normalized * flySpeed * Time.deltaTime, Space.World);
    }

    // Call this to start flying
    public void StartFlying()
    {
        if (!isFlying)
            StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        //I want a 1f delay before flying starts
        yield return new WaitForSeconds(1f);
        
        isFlying = true;

        yield return new WaitForSeconds(flyDuration);

        isFlying = false;

        // Optional: destroy or disable the bird
        // gameObject.SetActive(false);
        // Destroy(gameObject);
    }
}
