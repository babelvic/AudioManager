using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

//Own Namespaces
using AudioEngine;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine.Audio;
using UnityEngine.Timeline;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : UnityEditor.Editor
{
     AudioManager manager;

     //ReorderableList 

     private void OnEnable()
     {
         manager = target as AudioManager;
     }

     public override void OnInspectorGUI()
     {
         //Vertical Space for the Audio Manager
         using (new EditorGUILayout.VerticalScope())
         {
             //Horizontal Space for the General Management
             using (new EditorGUILayout.HorizontalScope())
             {
                 //A button for add a new track to the list
                 if (GUILayout.Button("Add Track"))
                 {
                     manager.tracks.Add(new AudioManager.AudioTrack());
                 }

                 GUILayout.Space(10f);

                 if (GUILayout.Button("Remove Track"))
                 {
                     manager.tracks.Remove(manager.tracks.ElementAt(manager.tracks.Count - 1));
                 }
             }

             GUIStyle greenStylePreset = new GUIStyle(GUI.skin.label);
             greenStylePreset.normal.textColor = new Color(.05f, .9f, .2f);

             GUIStyle blueStylePreset = new GUIStyle(GUI.skin.label);
             blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);

             //Vertical Space for the specific info of the track list
             using (new EditorGUILayout.VerticalScope())
             {
                 using (new EditorGUILayout.HorizontalScope("Box"))
                 {
                     try
                     {
                         //var track = serializedObject.FindProperty("tracks").GetArrayElementAtIndex(i);
                         EditorGUILayout.PropertyField(serializedObject.FindProperty("tracks"));
                         this.Repaint();
                     }
                     catch (InvalidOperationException e)
                     {
                         Debug.LogWarning($"Error with the serialization on the track: {e}");
                     }
                 }
             }
         }

     }
}