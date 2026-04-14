using DG.Tweening;
using Game.Utilities;
using UnityEngine;

namespace Game.Components
{
    public class PuzzleTextComponent : HoldableComponent
    {
        public SpriteRenderer MainRenderer;
        public SpriteRenderer CompletedRenderer;
        public GameObject Wings;

        public Range<Vector2> Boundary;
        public string ID;

        public bool IsCompleted;
        private void Start()
        {
            StartMoving();
        }

        public override void OnDragStart(Vector2 Position)
        {
            base.OnDragStart(Position);
            Wings.SetActive(false);
            transform.DOKill();
        }

        public override void OnFailed()
        {
            base.OnFailed();
            Wings.SetActive(true);
            StartMoving();
        }

        public override void OnCompleted()
        {
            base.OnCompleted();
            transform.DOKill(true);
            Wings.SetActive(false);
            MainRenderer.DOFade(0,2);
            IsCompleted = true;
            CompletedRenderer.DOFade(1, 2).OnComplete(() =>
            {
                FindObjectOfType<ParagraphGameComponent>().Container.SetActive(false);
                FindObjectOfType<MainGameplayComponent>().OptionSelection.StartGameplay();
                
            });
        }

        private void StartMoving()
        {
            var TargetPosition = new Vector2(Random.Range(Boundary.Min.x, Boundary.Max.x), Random.Range(Boundary.Min.y, Boundary.Max.y));
            transform.DOKill();
            transform.DOMove(TargetPosition, 4f).SetEase(Ease.Linear).OnComplete(StartMoving);
        }
    }
}