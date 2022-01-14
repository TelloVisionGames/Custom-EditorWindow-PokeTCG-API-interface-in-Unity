using Editor.Events;
using UnityEngine;

namespace Editor.Listeners
{
    public class RequestMadeEventListener : BaseCustomEventListener
    {
        
        public RequestMadeEventListener()
        {
            
            Debug.Log("Hello from RequestMadeEventListener");
        }

        public override void OnEventRaised()
        {
            Debug.Log("You have finally figured out how to raise an event yo");
        }
    }
}