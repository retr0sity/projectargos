using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Components
{
    public class UpdateComponent : MonoBehaviour
    {
        public Action ActionUpdate;

        private void Update()
        {
            ActionUpdate?.Invoke();
        }
    }
}