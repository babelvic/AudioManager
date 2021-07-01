using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AudioEngine;
using UnityEditor;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public List<AudioEvent> audioEvent;

    [System.Serializable]
    public class AudioEvent
    {
        public string SelectedEventName;
        public string TypeName;
    }

    private void Start()
    {
        foreach (var ae in audioEvent)
        {
            Type type = Type.GetType(ae.TypeName);
            EventInfo SelectedEvent = type.GetEvent(ae.SelectedEventName);

            Component selectedComponent = GetComponent(type);

            Delegate handler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType, AudioManager.instance , typeof(AudioManager).GetMethod("PlayTrack"));
            SelectedEvent.AddEventHandler(selectedComponent, handler);
        }
        if(FindObjectOfType<AudioManager>().gameObject != null) Debug.Log("enable");
    }
}
