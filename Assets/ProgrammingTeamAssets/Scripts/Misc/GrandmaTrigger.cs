using UnityEngine;
using System.Collections;

public class GrandmaTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public GameObject pot;   // assign in Inspector
    public Transform potSpawnPoint;       // assign where you want it to appear

    // Unity Example
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayAnimationAndSpawnPot());
        }
    }

    IEnumerator PlayAnimationAndSpawnPot()
    {
        animator.Play("GrandmaDropsPot");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        pot.transform.position = potSpawnPoint.position;
        pot.SetActive(true); // it becomes visible and physics starts
    }


}
