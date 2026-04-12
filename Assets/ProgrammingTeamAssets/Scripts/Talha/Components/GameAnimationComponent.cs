using Game.Scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class GameAnimationComponent : MonoBehaviour
{
    public Sprite Idle;
    public SpriteRenderer SpriteToAnimate;

    private Action ActionOnComplete;
    public List<AnimationData> AnimationDatas;

    public float Speed = 0.01f;
    public StringData Option;
    public GameObject Container;
    
    [ContextMenu("Test Animate")]
    public void TestAnimate()
    {
        Animate(Option.ID,null);
    }

    public void Animate(string ID, Action OnComplete)
    {
        ActionOnComplete = OnComplete;
        SpriteToAnimate.sprite = Idle;
        Container.SetActive(true);
        if (AnimationCoroutine != null)
        {
            StopCoroutine(AnimationCoroutine);
        }
        StartCoroutine(Animation(ID));
    }

    private Coroutine AnimationCoroutine;
    IEnumerator Animation(string ID)
    {
        var Data = AnimationDatas.FirstOrDefault(x => x.Option.ID == ID);

        for (int i = 0; i < Data.Sprites.Count; i++)
        {
            SpriteToAnimate.sprite = Data.Sprites[i];
            yield return new WaitForSeconds(Speed);
        }
        yield return new WaitForSeconds(3);
        ActionOnComplete?.Invoke();
        Container.SetActive(false);
    }

    [System.Serializable]
    public class AnimationData 
    {
        public StringData Option;
        public List<Sprite> Sprites;
    }
}
