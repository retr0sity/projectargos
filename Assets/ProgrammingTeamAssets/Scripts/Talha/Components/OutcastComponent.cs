using DG.Tweening;
using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class OutcastComponent : MonoBehaviour
{
    public List<Transform> Targets;
    public float MoveSpeed = 1f;

    private Sequence Sequence;
    public string DialogueText;
    public Transform MainObject;
    public Transform DialogueObject;

    private Vector3 StartPosition;
    private void Awake()
    {
        StartPosition = MainObject.localPosition;
    }
    private void OnEnable()
    {
        if (Sequence != null)
            Sequence.Kill();

        MainObject.localPosition = StartPosition;
        DialogueObject.transform.localScale = Vector3.zero;
        Sequence = DOTween.Sequence();

        for (int i = 0; i < Targets.Count; ++i)
        {
            Sequence.Append(MainObject.DOLocalMove(Targets[i].localPosition, MoveSpeed).SetEase(Ease.Linear));
        }
        Sequence.SetLoops(-1,LoopType.Yoyo);
        Sequence.Play();
    }

    public void ShowDialogue()
    {
        if (DialogueObject.transform.localScale.x > 0.1)
            return;
        DialogueObject.DOKill();
        DialogueObject.transform.localScale = Vector3.zero;
        DialogueObject.GetComponentInChildren<TextComponent>(true).SetupText(DialogueText);
        DialogueObject.DOScale(Vector3.one, 0.5f).OnComplete(() =>
        {
            DialogueObject.DOScale(Vector3.zero, 0.5f).SetDelay(3f);
        });
    }
}
