using DG.Tweening;
using UnityEngine;

namespace Game.Components
{
    public class ParagraphPageComponent : GameOptionComponent
    {
        public GameObject Container;
        protected override void Awake()
        {
            GetComponentInParent<OptionSelectionComponent>().ActionOptionChanged += OnOptionChanged;
        }

        public virtual void OnOptionChanged(string ID)
        {
            Container.SetActive(ID == Option.ID);
        }
    }
}