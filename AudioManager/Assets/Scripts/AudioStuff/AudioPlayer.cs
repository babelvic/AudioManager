using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AudioEngine;
using UnityEditor;
using UnityEditor.Profiling;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public List<AudioEvent> audioEvent;

    [System.Serializable]
    public class AudioEvent
    {
        public MonoBehaviour selectedScript;
        public MethodInfo selectedMethod;
    }
}
