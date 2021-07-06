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
    public List<string> allEventsNames;
    public List<string> allEventsTypes;
    
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
            for (var i = 0; i < allEventsNames.Count; i++)
            {
                Type type = Type.GetType(allEventsTypes[i]);
                EventInfo sEvent = type.GetEvent(allEventsNames[i]);

                Component selectedComponent = GetComponent(type);


                Delegate handler = Delegate.CreateDelegate(sEvent.EventHandlerType, AudioManager.instance, typeof(AudioManager).GetMethod("PlayTrack"));
                
                sEvent.AddEventHandler(selectedComponent, handler);
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
