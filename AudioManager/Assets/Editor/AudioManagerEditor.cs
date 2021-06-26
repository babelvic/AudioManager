using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEditor;

//Own Namespaces
using AudioEngine;
using NUnit.Framework.Constraints;
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
     
     private string dropdownLabelTracks;

     private void OnEnable()
     {
         manager = target as AudioManager;

         //Properties
         s_tracks = serializedObject.FindProperty(nameof(manager.tracks));

         #region ReorderableListTracks

         _reorderableTracks = new ReorderableList(serializedObject, s_tracks, true, true, false, false);
         _reorderableTracks.drawHeaderCallback = DrawHeaderTracks;
         _reorderableTracks.drawElementCallback = DrawListTracks;
         _reorderableTracks.drawFooterCallback = DrawFooterTracks;
         _reorderableTracks.drawNoneElementCallback = DrawBackgroundNoTracks;

         _reorderableTracks.elementHeightCallback = delegate(int index) {
             var element = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return 260 + margin;
             else return 20 + margin;
         };
         
         #endregion
         
     }

     public override void OnInspectorGUI()
     {
         //Vertical Space for the Audio Manager
         using (new EditorGUILayout.VerticalScope())
         {
             serializedObject.Update();
             
             //Horizontal Space for add and remove mixers
             using (new EditorGUILayout.HorizontalScope("Box"))
             {
                 if (GUILayout.Button("Add Mixer"))
                 {
                     manager.mixers.Add(new AudioManager.AudioTrackMixer());
                     manager.mixerGroupPopup.Add(string.Empty);
                 }

                 GUILayout.Space(10f);

                 if (GUILayout.Button("Remove Mixer"))
                 {
                     if(manager.mixers.Count >= 1) manager.mixers.Remove(manager.mixers.ElementAt(manager.mixers.Count - 1));
                     if(manager.mixerGroupPopup.Count >= 1) manager.mixerGroupPopup.RemoveAt(manager.mixerGroupPopup.Count - 1);
                 }
             }

             var buttonStyle = new GUIStyle(GUI.skin.button);
             buttonStyle.normal.textColor = new Color(.5f, .5f, 1);
             //Draw Mixers
             //_reorderableMixers.DoLayoutList();
             var  blueStylePreset = new GUIStyle(GUI.skin.box);
             blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
             var greenStylePreset = new GUIStyle(GUI.skin.button);
             greenStylePreset.normal.textColor = new Color(.05f, .9f, .2f);
             var redStylePreset = new GUIStyle(GUI.skin.button);
             greenStylePreset.normal.textColor = new Color(1f, .2f, .2f);
             
             for (int i = 0; i < manager.mixers.Count; i++)
             {
                 using (new EditorGUILayout.HorizontalScope("HelpBox"))
                 {
                     using (new EditorGUILayout.VerticalScope())
                     {
                         manager.mixers[i].dropdownMixer = EditorGUILayout.Foldout(manager.mixers[i].dropdownMixer, string.Empty);
                         EditorGUILayout.LabelField($"Mixer Group: {manager.mixers[i].name}", blueStylePreset);
                         if(manager.mixers[i].mixerGroup != null) manager.mixers[i].name = manager.mixers[i].mixerGroup.name;

                         if (manager.mixers[i].name != "")
                         {
                             manager.mixerGroupPopup[i] = manager.mixers[i].name;
                         }
                         else
                         {
                             manager.mixerGroupPopup[i] = "Default Mixer (You should put any name)";
                         }

                         if (manager.mixers[i].dropdownMixer)
                         {
                             manager.mixers[i].name = EditorGUILayout.TextField(manager.mixers[i].name);

                             manager.mixers[i].mixerGroup = (AudioMixerGroup) EditorGUILayout.ObjectField(manager.mixers[i].mixerGroup, typeof(AudioMixerGroup), true);
                         }
                         
                         if (GUILayout.Button("X", greenStylePreset))
                         {
                             manager.mixers.RemoveAt(i);
                             manager.mixerGroupPopup.RemoveAt(i);
                         }
                     }
                 }
             }
             
             EditorGUILayout.Space();
             
             //Horizontal Space for add and remove tracks
             using (new EditorGUILayout.HorizontalScope())
             {
                 if (GUILayout.Button("Add Track"))
                 {
                     manager.tracks.Add(new AudioManager.AudioTrack());
                     manager.mixerIndex.Add(0);
                 }

                 GUILayout.Space(10f);

                 if (GUILayout.Button("Remove Track"))
                 {
                     if(manager.tracks.Count - 1 >= 0) manager.tracks.RemoveAt(manager.tracks.Count-1);
                     if(manager.mixerIndex.Count - 1 >= 0) manager.mixerIndex.RemoveAt(manager.mixerIndex.Count-1);
                     // Debug.Log("id: " + manager.mixerIndex.Count);
                     // Debug.Log("track: " + manager.tracks.Count);
                     // Debug.Log("mixerGroupPopup: " + manager.mixerGroupPopup.Count);
                     // Debug.Log("mixerGroup: " + manager.mixers.Count);
                 }
             }

             _reorderableTracks.DoLayoutList();

             serializedObject.ApplyModifiedProperties();
         }
     }

     //Utility Function
     void FindMixerByID(int id, int index)
     {
         if (manager.mixers.Count >= 1)
         {
             if (manager.tracks[index].mixer != manager.mixers[id].mixerGroup)
             {
                 manager.tracks[index].mixer = manager.mixers[id].mixerGroup;
             }
         }
     }

     //Property Drawer for the tracks class
     public void DrawListTracks(Rect position, int index, bool isActive, bool isFocused)
    {
        SerializedProperty property = _reorderableTracks.serializedProperty.GetArrayElementAtIndex(index);

        position.width -= 34;
        position.height = 18;
        
        Rect dropdownRect = new Rect(position);
        dropdownRect.width = 10;
        dropdownRect.height = 10;
        dropdownRect.x += 10;
        dropdownRect.y += 5;
        
        property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, dropdownLabelTracks);
        
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
            var mixerField = property.FindPropertyRelative("mixer");

            EditorGUI.LabelField(fieldRect, "Mixer Group: ");

            var x = fieldRect.x;

            Rect mixerRect = new Rect(fieldRect.position, new Vector2((EditorGUIUtility.currentViewWidth * 0.2f), fieldRect.height));
            mixerRect.x += (EditorGUIUtility.currentViewWidth * 0.5f)-mixerRect.x;

            Rect popupRect = new Rect(fieldRect.position, new Vector2(200, 10));
            
            if (manager.tracks.Count - 1 >= index)
            {
                manager.mixerIndex[index] = EditorGUI.Popup(mixerRect, manager.mixerIndex[index], manager.mixerGroupPopup.ToArray());
                if (manager.mixerGroupPopup.Count - 1 < manager.mixerIndex[index])
                {
                    if(manager.mixerGroupPopup.Count - 1 >= 0) manager.mixerIndex[index] -= 1;
                    Debug.Log(manager.mixerIndex[index]);
                }
                FindMixerByID(manager.mixerIndex[index], index);
            }

            fieldRect.x = 0;
            fieldRect.x = EditorGUIUtility.currentViewWidth - 100;
            
            //Draw Loop
            manager.tracks[index].loop = EditorGUI.Toggle(fieldRect, manager.tracks[index].loop);

            fieldRect.x += 25;
            
            EditorGUI.LabelField(fieldRect, "Loop");
            
            fieldRect.x = x;
            
            Space(ref fieldRect);
            //Draw Clip
            EditorGUI.PropertyField(fieldRect, clipField);
            
            Space(ref fieldRect);
            //Draw Name
            EditorGUI.TextField(fieldRect, "Name" , nameField.stringValue);

            SerializedProperty priorityField = property.FindPropertyRelative("priority");
            SerializedProperty volumeField = property.FindPropertyRelative("volume");
            SerializedProperty pitchField = property.FindPropertyRelative("pitch");
            SerializedProperty SpatialBlendField = property.FindPropertyRelative("spatialBlend");

            var customLabel = new Label("LABEL");
            //Draw Values
            Space(ref fieldRect);
            EditorGUI.IntSlider(fieldRect, priorityField, 0, 256);
            Space(ref fieldRect);
            EditorGUI.Slider(fieldRect, volumeField, 0f, 1f);
            Space(ref fieldRect);
            EditorGUI.Slider(fieldRect, pitchField, -3f, 3);
            Space(ref fieldRect);
            EditorGUI.Slider(fieldRect, SpatialBlendField, 0f, 1f);
            Space(ref fieldRect);

            Rect buttonRect = new Rect(fieldRect.position, new Vector2(50, fieldRect.height));
            buttonRect.x += (EditorGUIUtility.currentViewWidth * 0.5f)-buttonRect.x;
            if (GUI.Button(buttonRect, "X"))
            {
                manager.tracks.Remove(manager.tracks.ElementAt(index));
                manager.mixerIndex.RemoveAt(index);
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
            if (GUI.Button(buttonRect, "X"))
            {
                manager.tracks.Remove(manager.tracks.ElementAt(index));
                manager.mixerIndex.RemoveAt(index);
            }
        }
        
        GetDropdownLabelTracks(index);
    }

     void GetDropdownLabelTracks(int index)
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
            dropdownLabelTracks = string.Empty;
        }
        else
        {
            var clipT = property.FindPropertyRelative(nameof(AudioManager.AudioTrack.clip));
            string clipName = string.Empty;
            if (clipT.objectReferenceValue != null)
            {
                clipName = ((AudioClip) clipT.objectReferenceValue).name;
            }
            else
            {
                clipName = string.Empty;
            }
            dropdownLabelTracks = clipName != string.Empty ? $"Track: {clipName}" : "Default Track";
        }
    }

     void DrawHeaderTracks(Rect rect)
    {
        var  blueStylePreset = new GUIStyle(GUI.skin.label);
        blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
        string name = "Audio Manager Tracks";
        EditorGUI.LabelField(rect, name, blueStylePreset);
    }
    
     void DrawFooterTracks(Rect rect)
    {
        var  blueStylePreset = new GUIStyle(GUI.skin.label);
        blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
        string name = "By @babelgames_es";
        EditorGUI.LabelField(rect, name, blueStylePreset);
    }
    
     void DrawBackgroundNoTracks(Rect rect)
    {
        var greenStylePreset = new GUIStyle(GUI.skin.label);
        greenStylePreset.normal.textColor = new Color(.05f, .9f, .2f);
        string name = "Add tracks for setting the audio in your game";
        EditorGUI.LabelField(rect, name, greenStylePreset);
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