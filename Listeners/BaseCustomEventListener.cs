using Editor.Events;
using Editor.Listeners;
using UnityEngine;
using UnityEngine.Events;

namespace Editor.Listeners {
    public abstract class BaseCustomEventListener : ICustomEventListener
    {
        public virtual void OnEventRaised() 
        {
           // Response?.Invoke();
        }
    }
}