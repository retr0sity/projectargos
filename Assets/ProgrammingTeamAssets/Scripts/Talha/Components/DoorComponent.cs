using DG.Tweening;
using Game.Components;
using System.Collections;
using UnityEngine;

public class DoorComponent : MonoBehaviour
{
    private DoorGameplayComponent mDoorGameplayComponent;
    private DoorGameplayComponent DoorGameplayComponent
    {
        get
        {
            if(mDoorGameplayComponent == null)
            {
                mDoorGameplayComponent = GetComponentInParent<DoorGameplayComponent>();
            }
            return mDoorGameplayComponent;
        }
    }

    public Transform DoorDialogue;
    public SpriteRenderer SpriteRenderer;
    public Sprite OpenedDoor;

    public void CheckDoorOpening()
    {
        Debug.Log("Checking Door");
        var KeyCount = DoorGameplayComponent.GetKeyCount();
        if(KeyCount.KeysCollected < KeyCount.KeysTotal)
        {
            if(DoorDialogueCoroutine == null)
            {
                SpriteRenderer.sprite = OpenedDoor;
                DoorDialogueCoroutine = StartCoroutine(DialogueRoutine());
            }
        }
    }

    private Coroutine DoorDialogueCoroutine;
    IEnumerator DialogueRoutine()
    {
        DoorDialogue.DOKill();
        DoorDialogue.DOScale(1, 0.5f);
        yield return new WaitForSeconds(3);
        DoorDialogue.DOScale(0,0.5f);
        DoorDialogueCoroutine = null;
    }
}
