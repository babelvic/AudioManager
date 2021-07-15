using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AudioEngine
{
    public class AudioPlayer : MonoBehaviour
    {
        public bool selectAllEvents;
        public bool configuration;
        public bool automatic;

        //All events
        public List<string> allEventsNames;
        public List<string> allEventsTypes;
        public bool allEventsSetted;

        //Each event
        public List<AudioEvent> audioEvent;

        public List<EventCreator> eventCreator;

        //Event creation
        [System.Serializable]
        public class EventCreator
        {
            public MonoBehaviour selectedScript;
            public MethodInfo selectedMethod;
            public int methodIndex;
            public string selectedTrack;

            public bool autoName = true;
            public string eventName;

            public string eventType;
        }

        [System.Serializable]
        public class AudioEvent
        {
            public int eventIndex;
            public string SelectedEventName;
            public string TypeName;
        }


        private void Awake()
        {
            if (automatic)
            {
                foreach (var ae in eventCreator)
                {
                    //Debug.Log(ae.eventName + " /// " + ae.eventType);
                    if (ae.eventType != string.Empty)
                    {
                        //Debug.Log(ae.eventType);
                        Type type = Type.GetType(ae.eventType);
                        EventInfo SelectedEvent = type.GetEvent(ae.eventName);

                        Component selectedComponent = GetComponent(type);

                        try
                        {
                            Delegate handler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType,
                                AudioManager.Instance, typeof(AudioManager).GetMethod("PlayTrack"));
                            SelectedEvent.AddEventHandler(selectedComponent, handler);
                        }
                        catch (NullReferenceException)
                        {
                            Debug.LogError($"{this.GetType().Name}:\nYou have the event maker but you don't implement it.\nRemove the event maker from the list or create it to subscribe");
                        } 
                    }
                }
            }
            else
            {
                if (selectAllEvents)
                {
                    //Subscribe all the events with a string as parameter by using a list of events info
                    for (var i = 0; i < allEventsNames.Count; i++)
                    {
                        Type type = Type.GetType(allEventsTypes[i]);
                        EventInfo sEvent = type.GetEvent(allEventsNames[i]);

                        Component selectedComponent = GetComponent(type);

                        Debug.Log(type + " | " + allEventsTypes[i]);

                        Delegate handler = Delegate.CreateDelegate(sEvent.EventHandlerType, AudioManager.Instance,
                            typeof(AudioManager).GetMethod("PlayTrack"));
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
                        
                        Debug.Log(type + " | " + ae.TypeName);

                        Delegate handler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType,
                            AudioManager.Instance, typeof(AudioManager).GetMethod("PlayTrack"));
                        SelectedEvent.AddEventHandler(selectedComponent, handler);
                    }
                }
            }

        }
    }
}
