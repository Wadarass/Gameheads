using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH
{
    public class Event
    {

    };

    public class EventSystem
    {
        private static EventSystem _instance = null;

        public static EventSystem instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventSystem();
                }
               
                return _instance;
            }
        }

        public delegate void EventDelegate<T>(T e) where T: Event;
        private delegate void EventDelegate(Event e);

        private Dictionary<System.Type, EventDelegate> eventDelegates = new Dictionary<System.Type, EventDelegate>();
        private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();

        public void AddListener<T>(EventDelegate<T> del) where T : Event
        {
            if (delegateLookup.ContainsKey(del))
            {
                return;
            }

            EventDelegate internalDelegate = (e) => del((T)e);
            delegateLookup[del] = internalDelegate;

            EventDelegate tempDel;
            if (eventDelegates.TryGetValue(typeof(T), out tempDel))
            {
                eventDelegates[typeof(T)] = tempDel += internalDelegate;
            }
            else
            {
                eventDelegates[typeof(T)] = internalDelegate;
            }
        }

        public void RemoveListener<T>(EventDelegate<T> del) where T: Event
        {
            EventDelegate internalDelegate;
            if (delegateLookup.TryGetValue(del, out internalDelegate))
            {
                EventDelegate tempDel;
                if (eventDelegates.TryGetValue(typeof(T), out tempDel))
                {
                    tempDel -= internalDelegate;
                    if (tempDel == null)
                    {
                        eventDelegates.Remove(typeof(T));
                    }
                    else
                    {
                        eventDelegates[typeof(T)] = tempDel;
                    }
                }

                delegateLookup.Remove(del);
            }
        }

        public void RaiseEvent(Event eventArguments)
        {
            EventDelegate del;
            if (eventDelegates.TryGetValue(eventArguments.GetType(), out del))
            {
                del.Invoke(eventArguments);
            }
        }
    }
}
