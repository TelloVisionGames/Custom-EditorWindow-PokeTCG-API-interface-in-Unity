using UnityEngine;

namespace Editor.Events
{
    public class RequestMadeEvent : BaseCustomEvent
    {
        public RequestMadeEvent()
        {
            Debug.Log("Hello from RequestMadeEvent()");
        }
    }
}