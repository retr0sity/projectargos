using UnityEngine;
using DG.Tweening;
using Game.Components;
using System.Collections;
using System.Collections.Generic;
//using Game.Controllers;
using Game.Managers;

public class HoldableComponent : BaseDraggableComponent
{
    //protected LevelAudioController LevelAudioController => DependencyManager.Instance.LevelAudioController;

    public bool DoRotate;

    public bool IsInteractable
    {
        get => Collider2D.enabled;
        set => Collider2D.enabled = value;
    }

    private Collider2D mCollider2D;
    public Collider2D Collider2D
    {
        get
        {
            if (mCollider2D == null)
            {
                mCollider2D = GetComponent<Collider2D>();
            }
            return mCollider2D;
        }
    }

    public HolderComponent Holder;
    public float InteractionDistance = 1;

    public override void OnDragEnd()
    {
        base.OnDragEnd();
        if (Vector2.Distance(transform.position, Holder.transform.position) < InteractionDistance)
        {
            IsInteractable = false;
            transform.DOMove(Holder.transform.position, 0.2f).OnUpdate(() =>
            {
                OnUpdate();
            }).OnComplete(() =>
            {
                //LevelAudioController.Play("PuzzleCorrect");
                OnCompleted();
            });

            if(DoRotate)
            {
                transform.DORotateQuaternion(Holder.transform.rotation,0.2f);
            }
        }
        else
        {
            OnFailed();
        }
    }

    public virtual void OnCompleted()
    {

    }

    public virtual void OnFailed()
    {

    }

    public virtual void OnUpdate()
    {

    }
}
