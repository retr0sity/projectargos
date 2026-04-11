using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Controllers
{
    public class InterfaceController : MonoBehaviour
    {
        public string CurrentInterface;
        public Action<string> ActionSwitchInterface;

        public void SwitchInterface(string SwitchInterface)
        {
            CurrentInterface = SwitchInterface;
            ActionSwitchInterface?.Invoke(CurrentInterface);
        }
    }
}