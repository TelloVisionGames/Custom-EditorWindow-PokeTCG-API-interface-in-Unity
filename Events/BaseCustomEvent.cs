using System.Collections.Generic;
using Editor.Listeners;
using UnityEngine;

namespace Editor.Events {
    public abstract class BaseCustomEvent{
        private readonly List<ICustomEventListener> listeners = new List<ICustomEventListener>();

        public void Raise() {
            for(int i = listeners.Count - 1; i >= 0; i -= 1) {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(ICustomEventListener listener) {
            if(!listeners.Contains(listener)) {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(ICustomEventListener listener) {
            while(listeners.Contains(listener)) {
                listeners.Remove(listener);
            }
        }
    }
}