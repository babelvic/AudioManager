using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AudioEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    public bool selectAllEvents;
    
    //All events
    public List<EventInfo> allEvents;
    
    //Each event
    public List<AudioEvent> audioEvent;

    [System.Serializable]
    public class AudioEvent
    {
        public string SelectedEventName;
        public string TypeName;
    }

    private void Start()
    {
        if (selectAllEvents)
        {
            //Subscribe all the events with a string as parameter by using a list of events info
            foreach (var mEvent in allEvents)
            {
                string TypeName = mEvent.DeclaringType.AssemblyQualifiedName;
                
                Delegate handler = Delegate.CreateDelegate(mEvent.EventHandlerType, AudioManager.instance, typeof(AudioManager).GetMethod("PlayTrack"));

                Type type = Type.GetType(TypeName);
                Component selectedComponent = GetComponent(type);
                
                mEvent.AddEventHandler(selectedComponent, handler);
            }
        }
        else
        {
            //Subscribe by selecting each event that you want to subscribe it
            foreach (var ae in audioEvent)
            {
                Type type = Type.GetType(ae.TypeName);
                EventInfo SelectedEvent = type.GetEvent(ae.SelectedEventName);

                Component selectedComponent = GetComponent(type);

                Delegate handler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType, AudioManager.instance , typeof(AudioManager).GetMethod("PlayTrack"));
                SelectedEvent.AddEventHandler(selectedComponent, handler);
            }
        }
    }
}
