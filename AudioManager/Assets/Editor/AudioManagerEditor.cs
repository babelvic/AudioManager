using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(AudioManagerEditor))]public class AudioManagerEditor : Editor
{
     AudioManagerEditor manager;
     void OnEnable()
     {
         manager = target as AudioManagerEditor;
     }

     public override void OnInspectorGUI()
     {
         base.OnInspectorGUI();
     }

}