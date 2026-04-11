using UnityEngine;

namespace Game.Components
{
    public class MainGameOptionComponent : GameOptionComponent
    {
        public GameObject Selected;
        public GameObject UnSelected;
        public override void OnOptionChanged(string ID)
        {
            Debug.Log(ID);
            Selected.SetActive(ID == this.Option.ID);
            UnSelected.SetActive(ID != this.Option.ID);
        }
    }
}