using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
     AudioPlayer manager;
     
     //List<List<string>> scriptEvents = new List<List<string>>();

     public override void OnInspectorGUI()
     {
         manager = target as AudioPlayer;

         List<MonoBehaviour> scripts = manager.GetComponents<MonoBehaviour>().Where(s => s.GetType().Name != manager.GetType().Name).ToList();
         
         if (scripts.Count > 0)
         {
             string[] scriptNames = scripts.Select(s => s.GetType().Name).ToArray();
             
             int scriptIndex = EditorGUILayout.Popup("Script", scripts.ToList().IndexOf(manager.selectedScript), scriptNames);
             if (scriptIndex >= 0) manager.selectedScript = scripts[scriptIndex];
             else if (scripts.Count > 0) manager.selectedScript = scripts[0];

            
             List<MethodInfo> methods = manager.selectedScript.GetType().GetMethods().Where(m => m.DeclaringType == manager.selectedScript.GetType()).ToList();
             
             if (methods.Count > 0)
             {
                 string[] methodNames = methods.Select(m => m.Name).ToArray();

                 int methodIndex = EditorGUILayout.Popup("Method", methods.ToList().IndexOf(manager.selectedMethod), methodNames);
                 if (methodIndex >= 0) manager.selectedMethod = methods[methodIndex];
                 else manager.selectedMethod = methods[0];
             }
         }
     }
     
     
     // scriptEvents.Add(scripts.Select(s => s.GetType().Name).ToList());
     //
     //     for (int i = 0; i < scripts.Count; i++)
     // {
     //     var methodss = scripts.GetType().GetMethods().Where(m => m.DeclaringType == manager.audioEvent.selectedScript.GetType()).ToList();
     //     if (methodss.Count > 0)
     //     {
     //         for (int j = 0; j < methodss.Count; j++)
     //         {
     //             scriptEvents[i].Add(methodss[j].Name);
     //         }
     //     }
     // }

}