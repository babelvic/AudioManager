using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

//Own Namespaces
using AudioEngine;
using UnityEditorInternal;
using UnityEngine.Audio;
using UnityEngine.Timeline;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : UnityEditor.Editor
{
     AudioManager manager;

     private List<SerializedProperty> s_trackers = new List<SerializedProperty>();
     //ReorderableList 

     private void OnEnable()
     {
         manager = target as AudioManager;

         for (int i = 0; i < manager.tracks.Count; i++)
         {
             SerializedProperty track = serializedObject.FindProperty("tracks").GetArrayElementAtIndex(i);
             
             s_trackers.Add(track);
         }
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
                 /*
                 for (int i = 0; i < manager.tracks.Count; i++)
                 {
                     //Vertical Space for serialize all the info of each track in the list
                     using (new EditorGUILayout.VerticalScope("Box"))
                     {
                         var trackAtIndex = manager.tracks.ElementAt(i);

                         //Horizontal Space for serialize all the info of each track in the list
                         using (new EditorGUILayout.HorizontalScope("Box"))
                         {
                             AudioClip oldClip = trackAtIndex.clip;

                             trackAtIndex.clip = EditorGUILayout.ObjectField(trackAtIndex.clip, typeof(AudioClip), true) as AudioClip;

                             if (trackAtIndex.clip != null || trackAtIndex.clip != oldClip)
                             {
                                 trackAtIndex.name = trackAtIndex.clip.name;
                                 this.Repaint();
                             }

                             EditorGUILayout.Separator();

                             trackAtIndex.name = EditorGUILayout.TextField(trackAtIndex.name);

                             EditorGUILayout.Separator();
                             //LOOP
                             EditorGUILayout.BeginHorizontal();
                             GUILayout.Label(@"Track Loop", greenStylePreset);

                             EditorGUILayout.Space(10f);

                             trackAtIndex.loop = EditorGUILayout.Toggle(trackAtIndex.loop);
                             EditorGUILayout.EndHorizontal();
                             //LOOP
                         }

                         GUILayout.Space(5f);

                         //Audio Mixer
                         EditorGUILayout.BeginHorizontal();
                         GUILayout.Label($"Mixer of {trackAtIndex.name}", greenStylePreset);

                         EditorGUILayout.Separator();

                         trackAtIndex.mixer =
                             EditorGUILayout.ObjectField(trackAtIndex.mixer, typeof(AudioMixer), true) as AudioMixer;
                         EditorGUILayout.EndHorizontal();
                         //Audio Mixer

                         GUILayout.Space(5f);

                         //Volume
                         EditorGUILayout.BeginHorizontal();
                         GUILayout.Label("Volume", blueStylePreset);

                         EditorGUILayout.Separator();
                         trackAtIndex.volume = 1f;
                         trackAtIndex.volume = EditorGUILayout.Slider(trackAtIndex.volume, 0, 1);
                         EditorGUILayout.EndHorizontal();
                         //Volume

                         GUILayout.Space(5f);

                         //Pitch
                         EditorGUILayout.BeginHorizontal();
                         GUILayout.Label("Pitch", blueStylePreset);

                         EditorGUILayout.Separator();
                         trackAtIndex.pitch = 1f;
                         trackAtIndex.pitch = EditorGUILayout.Slider(trackAtIndex.pitch, -3f, 3f);
                         EditorGUILayout.EndHorizontal();
                         //Pitch
                     }

                     DrawUILine(new Color(.1f, .8f, .8f));
                 }
                 */
                 
                 for (int i = 0; i < s_trackers.Count; i++)
                 {
                     EditorGUILayout.PropertyField(s_trackers.ElementAt(i));
                 }
             }

         }

     }

     public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
     {
         Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
         r.height = thickness;
         r.y+=padding/2;
         r.x-=2;
         r.width +=6;
         EditorGUI.DrawRect(r, color);
     }

}