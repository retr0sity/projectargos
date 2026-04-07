using System;
using UnityEngine;
using Game.Interface;
using System.Collections;
using System.Collections.Generic;

namespace Game.Components
{
    public class TimerComponent : MonoBehaviour,IUpdateSubscriber
    {
        public float TickTime = 0.01f;
        private float TimeElapsed;
        public Action<float> ActionTick;
        public bool IsUpdateRunning { get; set; }
        public IUpdateSubscriber UpdateSubscriber => this;

        private void Start()
        {
            UpdateSubscriber.Subscribe();
        }

        public void OnUpdate()
        {
            TimeElapsed += Time.deltaTime;
            if (TimeElapsed >= TickTime)
            {
                ActionTick?.Invoke(TickTime);
                TimeElapsed = 0;
            }
        }

        public void OnDestroy()
        {
            UpdateSubscriber.UnSubscribe();
        }
    }
}