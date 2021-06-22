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
using UnityEngine.UIElements;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : UnityEditor.Editor
{
     AudioManager manager;

     private SerializedProperty s_tracks;

     private ReorderableList _reorderableTracks;
     
     string dropdownLabel;

     private void OnEnable()
     {
         manager = target as AudioManager;

         s_tracks = serializedObject.FindProperty(nameof(manager.tracks));

         _reorderableTracks = new ReorderableList(serializedObject, s_tracks, true, true, false, false);
         _reorderableTracks.drawHeaderCallback = DrawHeader;
         _reorderableTracks.drawElementCallback = DrawListItems;
         
         _reorderableTracks.elementHeightCallback = delegate(int index) {
             var element = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return 230 + margin;
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

             serializedObject.Update();
             
             _reorderableTracks.DoLayoutList();

             serializedObject.ApplyModifiedProperties();
         }
     }
     
     
    public void DrawListItems(Rect position, int index, bool isActive, bool isFocused)
    {
        SerializedProperty property = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(index);
        
        GUIStyle blueStylePreset = new GUIStyle(GUI.skin.label);
        blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
        
        position.width -= 34;
        position.height = 18;
        
        Rect dropdownRect = new Rect(position);
        dropdownRect.width = 10;
        dropdownRect.height = 10;
        dropdownRect.x += 10;
        dropdownRect.y += 5;
        
        property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, dropdownLabel);
        
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
            Space(ref fieldRect, 20f);
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

            var customLabel = new Label("LABEL");
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

            Rect buttonRect = new Rect(fieldRect.position, new Vector2(50, fieldRect.height));
            buttonRect.x += (EditorGUIUtility.currentViewWidth * 0.5f)-buttonRect.x;
            if (GUI.Button(buttonRect, "-"))
            {
                manager.tracks.Remove(manager.tracks.ElementAt(index));
            }
            Space(ref fieldRect, 15);
            DrawUILine(fieldRect.x, fieldRect.y);
            Space(ref fieldRect);
        }
        else
        {
            Rect buttonRect = new Rect(dropdownRect.position, new Vector2(50, 20));
            buttonRect.y -= 5;
            buttonRect.x += (EditorGUIUtility.currentViewWidth - (buttonRect.x * 3));
            if (GUI.Button(buttonRect, "-"))
            {
                manager.tracks.Remove(manager.tracks.ElementAt(index));
            }
        }
        
        GetDropdownLabel(index);
    }

    void GetDropdownLabel(int index)
    {
        int i = index;

        i++;

        if (i > _reorderableTracks.count - 1)
        {
            i = 0;
        }
        
        SerializedProperty property = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(i);

        if (property.isExpanded)
        {
            dropdownLabel = string.Empty;
        }
        else
        {
            var clipT = property.FindPropertyRelative(nameof(AudioManager.AudioTrack.clip));
            string clipName = string.Empty;
            if (clipT.objectReferenceValue != null)
            {
                clipName = ((AudioClip) clipT.objectReferenceValue).name;
                dropdownLabel = clipName != string.Empty ? $"Track: {clipName}" : "Default Track";
            }
        }
    }

    void DrawHeader(Rect rect)
    {
        string name = "Audio Manager Tracks";
        EditorGUI.LabelField(rect, name);
    }
    
    public void Space(ref Rect pos, float space = 30f)
    {
        pos.y += space;
    }
    
    public static void DrawUILine(float posX, float posY, float thickness = 38, float padding = 30)
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