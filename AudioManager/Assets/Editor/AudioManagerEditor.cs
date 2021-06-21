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

     private SerializedProperty s_tracks;

     private ReorderableList _reorderableTracks;

     private void OnEnable()
     {
         manager = target as AudioManager;

         s_tracks = serializedObject.FindProperty(nameof(manager.tracks));

         _reorderableTracks = new ReorderableList(serializedObject, s_tracks, true, true, false, false);
         _reorderableTracks.drawElementCallback = DrawListItems;
         
         _reorderableTracks.elementHeightCallback = delegate(int index) {
             var element = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return 200 + margin;
             else return 20 + margin;
         };
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

             serializedObject.Update();
             
             //Vertical Space for the specific info of the track list
             /*
             using (new EditorGUILayout.VerticalScope("Box"))
             {
                 try
                 {
                     for (int i = 0; i < manager.tracks.Count; i++)
                     {
                         EditorGUILayout.BeginHorizontal("Box");
                         var track = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(i);
                         EditorGUILayout.PropertyField(track);
                         EditorGUILayout.EndHorizontal();
                     }
                 }
                 catch (InvalidOperationException e)
                 {
                     Debug.LogWarning($"Error with the serialization on the track: {e}");
                 }
             }
             */
             
             
             _reorderableTracks.DoLayoutList();
             
             serializedObject.ApplyModifiedProperties();
         }

     }
     
     
    public void DrawListItems(Rect position, int index, bool isActive, bool isFocused)
    {
        SerializedProperty property = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(index);
        
        position.width -= 34;
        position.height = 18;

        Rect dropdownRect = new Rect(position);
        dropdownRect.width = 10;
        dropdownRect.height = 10;
        dropdownRect.x += 10;
        dropdownRect.y += 5;

        property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, string.Empty);

        position.x += 50;
        position.width -= 15;


        Rect fieldRect = new Rect(position);

        SerializedProperty clipField = property.FindPropertyRelative("clip");

        SerializedProperty nameField = property.FindPropertyRelative(nameof(AudioManager.AudioTrack.name));

        if (clipField.objectReferenceValue != null)
        {
            nameField.stringValue = ((AudioClip) clipField.objectReferenceValue).name;
        }


        if (property.isExpanded)
        {
            Space(ref fieldRect, 5f);
            //Draw Clip
            EditorGUI.PropertyField(fieldRect, clipField);
    
            Space(ref fieldRect);
            //Draw Name
            EditorGUI.TextField(fieldRect, "Name" , nameField.stringValue);

            var mixerField = property.FindPropertyRelative("mixer");

            var loopField = property.FindPropertyRelative("loop");
        
            SerializedProperty priorityField = property.FindPropertyRelative("priority");
            SerializedProperty volumeField = property.FindPropertyRelative("volume");
            SerializedProperty pitchField = property.FindPropertyRelative("pitch");
            SerializedProperty SpatialBlendField = property.FindPropertyRelative("spatialBlend");

            //Draw Values
            Space(ref fieldRect);
            EditorGUI.Slider(fieldRect, priorityField, 0f, 256f);
            Space(ref fieldRect);
            EditorGUI.Slider(fieldRect, volumeField, 0f, 1);
            Space(ref fieldRect);
            EditorGUI.Slider(fieldRect, pitchField, -3f, 3);
            Space(ref fieldRect);
            EditorGUI.Slider(fieldRect, SpatialBlendField, 0f, 1f);
            Space(ref fieldRect);
            
            DrawUILine(fieldRect.x, fieldRect.y);
            Space(ref fieldRect);
        }
    }

    public void Space(ref Rect pos, float space = 30f)
    {
        pos.y += space;
    }
    
    public static void DrawUILine(float posX, float posY, float thickness = 28, float padding = 30)
    {
        Rect r = new Rect(posX, posY, thickness, padding);
        r.width = EditorGUIUtility.currentViewWidth;
        r.height = 2;
        r.y+=padding * 0.3f;
        r.x-=70;
        r.width -= thickness;
        EditorGUI.DrawRect(r, Color.cyan);
    }
}