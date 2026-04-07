using UnityEngine;
using Game.Managers;
using Game.Components;
using System.Collections;
using System.Collections.Generic;

namespace Game.Interface
{
    public interface IUpdateSubscriber
    {
        public IUpdateSubscriber UpdateSubscriber { get; }
        private UpdateComponent UpdateComponent => DependencyManager.Instance.UpdateComponent;

        public void OnUpdate();
        public void OnDestroy();

        public bool IsUpdateRunning
        {
            get;
            set;
        }

        public void Subscribe()
        {
            if (!IsUpdateRunning)
            {
                UpdateComponent.ActionUpdate += OnUpdate;
                IsUpdateRunning = true;
            }
        }

        public void UnSubscribe()
        {
            if (IsUpdateRunning && UpdateComponent)
            {
                UpdateComponent.ActionUpdate -= OnUpdate;
                IsUpdateRunning = false;
            }
        }
    }
}