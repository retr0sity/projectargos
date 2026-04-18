using System;
using UnityEngine;
using Game.Interface;
using System.Collections;
using System.Collections.Generic;

namespace Game.Components
{
    public class TimerComponent : MonoBehaviour
    {
        public float TickTime = 0.01f;
        private float TimeElapsed;
        public Action<float> ActionTick;


        public void FixedUpdate()
        {
            TimeElapsed += Time.deltaTime;
            if (TimeElapsed >= TickTime)
            {
                ActionTick?.Invoke(TickTime);
                TimeElapsed = 0;
            }
        }
    }
}