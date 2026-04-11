using System.Linq;
using UnityEngine;
using Game.Managers;
using Game.Extensions;
using Game.Controllers;
using Game.Scriptables;
using System.Collections.Generic;

namespace Game.Components
{
    public class GameInterfaceComponent : MonoBehaviour
    {
        public List<StringData> GameInterfaces;
        public List<GameObject> Containers;

        private InterfaceController InterfaceController => DependencyManager.Instance.InterfaceController;

        void Start()
        {
            InterfaceController.ActionSwitchInterface += SwitchInterface;
        }

        void SwitchInterface(string SwitchInterface)
        {
            bool Status = GameInterfaces.Any(x => x.ID == SwitchInterface);
            Containers.SetActiveOptimized(Status);
        }

       
    }
}
