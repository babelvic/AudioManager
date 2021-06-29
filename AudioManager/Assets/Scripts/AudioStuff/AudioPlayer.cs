using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioEngine;
using UnityEditor;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioManager m_instanceAudioManager;

    public List<AudioEvent> audioEventList;
    public MonoBehaviour selectedScript;

    private void Start()
    {
        GetFirstScriptAttached();
    }

    private void GetFirstScriptAttached()
    {
        var attachedScript = GetComponents<MonoBehaviour>();
        string[] scriptNames = attachedScript.Select(s => s.GetType().Name).ToArray();

        foreach (var a in audioEventList)
        {
            if (a.scriptReference == null)
            {
                if (attachedScript.FirstOrDefault().GetType() == typeof(MonoScript))
                {
                    attachedScript.FirstOrDefault();
                    //a.scriptReference = attachedScript.FirstOrDefault();
                }
                foreach (var sn in scriptNames)
                {
                    Debug.Log(sn);
                }
            }
        }

    }


    [System.Serializable]
    public class AudioEvent
    {
        public MonoBehaviour scriptReference;
    }

    
}
