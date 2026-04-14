using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace Game.Components
{
    public class MenuGameOptionComponent : GameOptionComponent
    {
        public GameObject Selected;
        public GameObject UnSelected;
        protected override void Awake()
        {
            GetComponentInParent<OptionSelectionComponent>().ActionOptionChanged += OnOptionChanged;
        }
        public override void OnOptionChanged(string ID)
        {
            Selected.SetActive(ID == this.Option.ID);
            UnSelected.SetActive(ID != this.Option.ID);
        }
    }
}